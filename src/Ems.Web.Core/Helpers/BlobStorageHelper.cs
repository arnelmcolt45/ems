using Abp.Domain.Repositories;
using Abp.IO.Extensions;
using Ems.Storage;
using System.Linq;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Ems.Web.Helpers
{
    public class BlobStorageHelper : EmsServiceBase
    {
        //private readonly CloudBlobClient _cloudBlobClient;
        //private readonly CloudBlobContainer _container;
        //private readonly Uri _blobServiceEndpoint = new Uri("https://aelfilesandimages.blob.core.windows.net/");
        //private readonly string _containerName = "aelcontainer";
        private readonly AzureStorageConfiguration _storageConfig;

        public BlobStorageHelper(AzureStorageConfiguration storageConfig)
        {
            _storageConfig = storageConfig;
            //var credentials = new StorageCredentials("aelfilesandimages", "tsQkUeS4/EhdpnDL81RDfPLJLslYAoO/ObiB+9d8XItDB+tcWlHaUXPe0fLkKmLYxHIh06qUgeXxpWCKQf87fA==");
            //_cloudBlobClient = new CloudBlobClient(_blobServiceEndpoint, credentials); 
            //_container = _cloudBlobClient.GetContainerReference(_containerName);
        }

        private static string GenerateFileName()
        {
            return $"{DateTime.UtcNow.ToString("yyyyMMddhhmmssffff")}-{Guid.NewGuid().ToString().Substring(0, 8)}";
        }

        private CloudBlobContainer GetCloudBlobContainer(AzureStorageConfiguration config)
        {
            var credentials = new StorageCredentials(config.AccountName, config.KeyValue);
            var cloudBlobClient = new CloudBlobClient(new Uri(config.BlobStorageEndpoint), credentials); 
            var container = cloudBlobClient.GetContainerReference(config.ContainerName);

            return container;
        }

        public Uri GetResourceUriWithSas(string resourcePath)
        {
            var container = GetCloudBlobContainer(_storageConfig);

            if (!string.IsNullOrWhiteSpace(resourcePath))
            {
                var sasPolicy = new SharedAccessBlobPolicy()
                {
                    Permissions = SharedAccessBlobPermissions.Read,
                    SharedAccessStartTime = DateTime.Now.AddMinutes(-1),
                    SharedAccessExpiryTime = DateTime.Now.AddMinutes(30)
                };

                CloudBlockBlob blob = container.GetBlockBlobReference(resourcePath);
                string sasToken = blob.GetSharedAccessSignature(sasPolicy);

                return new Uri($"{_storageConfig.BlobStorageEndpoint}{_storageConfig.ContainerName}/{resourcePath}{sasToken}");
            }

            return null;
        }

        public async Task<string> SaveAttachment(IFormFile aFile, string subDirectory)
        {
            var container = GetCloudBlobContainer(_storageConfig);

            string id = $"{subDirectory}/{GenerateFileName()}{Path.GetExtension(aFile.FileName)}";
            CloudBlockBlob blob = container.GetBlockBlobReference(id);
            blob.Properties.ContentType = aFile.ContentType;

            byte[] fileBytes;
            using (var stream = aFile.OpenReadStream())
            {
                fileBytes = stream.GetAllBytes();
            }
            Stream strm = new MemoryStream(fileBytes);

            await blob.UploadFromStreamAsync(strm);

            Array.Clear(fileBytes, 0, fileBytes.Length);
            strm.Dispose();

            return id;
        }
    }
}
