using System;

namespace EventHorizon.Identity.AuthServer.Captcha.Models
{
    public class CaptchaSettings
    {
        public string SiteKey { get; }
        public string Secret { get; }

        public CaptchaSettings(
            string siteKey, 
            string secret
        )
        {
            SiteKey = siteKey;
            Secret = secret;
        }
    }
}
