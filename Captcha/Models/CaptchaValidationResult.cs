using Newtonsoft.Json;
using System;

namespace EventHorizon.Identity.AuthServer.Captcha.Models
{
    public class CaptchaValidationResult
    {
        public bool Success { get; set; }
        public decimal Score { get; set; }
        public string Action { get; set; }
        [JsonProperty("challenge_ts")]
        public DateTimeOffset ChallengeTimestamp { get; set; }
        public string Hostname { get; set; }
        [JsonProperty("error-codes")]
        public string[] ErrorCodes { get; set; }
    }
}
