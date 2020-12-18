using System.Security.Claims;

namespace Ems.Web.Models.Xero
{
    public static class ClaimsPrincipalExtensions
    {
        public static string XeroUserId(this ClaimsPrincipal claims)
        {
            return claims.FindFirstValue("xero_userid");
        }
    }
}
