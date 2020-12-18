using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Abp.Web.Models;
using Ems.Storage;
using Ems.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ems.Web.Controllers
{
    [AbpMvcAuthorize]
    public class AttachmentsController : EmsControllerBase
    {
        //private readonly BlobStorageHelper _blobStorageService;
        private readonly IRepository<AzureStorageConfiguration> _storageConfigRepository;
        public static int MaxAttachmentDefaultFileSize = 26214400; //25MB
        public static int MaxAttachmentVideoFileSize = 104857600; //100MB

        public AttachmentsController(IRepository<AzureStorageConfiguration> storageConfigRepository)
        {
            _storageConfigRepository = storageConfigRepository;
        }

        public static List<string> AttachmentSupportedFileFormats
        {
            get
            {
                return new List<string> {
                    ".png", ".jpg", ".jpeg", ".bmp", ".svg", ".psd", ".mp4", ".mpeg", ".mov", ".xls", ".xlsx", ".csv", ".doc", ".docx", ".ppt", ".pptx", ".pdf"
                };
            }
        }

        public static List<string> AttachmentSupportedVideoFileFormats
        {
            get
            {
                return new List<string> {
                    ".mp4", ".mpeg", ".mov"
                };
            }
        }

       
        
        [HttpPost]
        public async Task<JsonResult> UploadAttachment()
        {
            var storageConfig = GetAzureStorageConfiguration();
            var _blobStorageService = new BlobStorageHelper(storageConfig);

            try
            {
                string uploadFilePath = string.Empty;
                string subDirectory = Request.Form["subDirectory"];
                IFormFile attachedFile = Request.Form.Files.FirstOrDefault();

                //Check input
                if (attachedFile == null)
                {
                    throw new UserFriendlyException(L("File_Empty_Error"));
                }
                else
                {
                    string fileExtn = Path.GetExtension(attachedFile.FileName)?.ToLower();
                    if (!string.IsNullOrWhiteSpace(fileExtn) && AttachmentSupportedFileFormats.Contains(fileExtn))
                    {
                        if ((AttachmentSupportedVideoFileFormats.Contains(fileExtn) && attachedFile.Length > MaxAttachmentVideoFileSize) || (attachedFile.Length > MaxAttachmentDefaultFileSize‬))
                        {
                            throw new UserFriendlyException(L("File_SizeLimit_Error"));
                        }
                        else
                        {
                            uploadFilePath = await _blobStorageService.SaveAttachment(attachedFile, subDirectory);
                        }
                    }
                    else
                    {
                        throw new UserFriendlyException(L("File_Invalid_Type_Error"));
                    }
                }

                if (!string.IsNullOrWhiteSpace(uploadFilePath))
                {
                    //uploadFilePath = Path.Combine(BlobContainerName, uploadFilePath);

                    return Json(new AjaxResponse(new
                    {
                        filename = attachedFile.FileName,
                        blobPath = uploadFilePath,
                        bolbFolder = Path.GetDirectoryName(uploadFilePath),
                        bolbId = Path.GetFileName(uploadFilePath)
                    }));
                }
                else
                {
                    throw new UserFriendlyException(L("File_Upload_Error"));
                }
            }
            catch (UserFriendlyException ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpGet]
        public string GetAttachmentURI(string resourcePath)
        {
            var storageConfig = GetAzureStorageConfiguration();
            var _blobStorageService = new BlobStorageHelper(storageConfig);

            return _blobStorageService.GetResourceUriWithSas(resourcePath)?.ToString();
        }

        private AzureStorageConfiguration GetAzureStorageConfiguration()
        {
            var storageConfig = _storageConfigRepository.GetAll().Where(a => a.Service == "Attachments").FirstOrDefault();  // TODO: Make this Tenant Specific, somehow!
            if (storageConfig == null)
            {
                throw new Exception("Cannot find Azure Storage Configuration for Attachments!");
            }
            return storageConfig;
        }
    }
}