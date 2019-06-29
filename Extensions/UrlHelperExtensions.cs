using System;
using EventHorizon.Identity.AuthServer.Identity;

namespace Microsoft.AspNetCore.Mvc
{
    public static class UrlHelperExtensions
    {
        public static string EmailConfirmationLink(
            this IUrlHelper urlHelper, 
            string userId, 
            string code, 
            string scheme
        )
        {
            return urlHelper.Page(
                pageName: "/Account/ConfirmEmail",
                pageHandler: "/Register/Index",
                values: new { userId, code },
                protocol: scheme
            );
        }

        public static string ResetPasswordCallbackLink(
            this IUrlHelper urlHelper, 
            string userId, 
            string code, 
            string scheme)
        
        {
            return urlHelper.Page(
                pageName: "/Account/ResetPassword",
                pageHandler: "/Register/Index",
                values: new { userId, code },
                protocol: scheme
            );
        }
    }
}
