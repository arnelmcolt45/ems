using Newtonsoft.Json;
using System.IO;
using Xero.NetStandard.OAuth2.Token;

namespace Ems.Utilities
{
    public static class TokenUtilities
    {
        public static void StoreToken(XeroOAuth2Token xeroToken)
        {
            string serializedXeroToken = JsonConvert.SerializeObject(xeroToken);

            System.IO.File.WriteAllText("./xerotoken.txt", serializedXeroToken);
        }

        public static XeroOAuth2Token GetStoredToken()
        {
            string serializedXeroToken = System.IO.File.ReadAllText("./xerotoken.txt");
            var xeroToken = JsonConvert.DeserializeObject<XeroOAuth2Token>(serializedXeroToken);

            return xeroToken;
        }

        public static bool TokenExists()
        {
            string serializedXeroTokenPath = "./xerotoken.txt";
            bool fileExist = File.Exists(serializedXeroTokenPath);

            return fileExist;
        }
    }
}
