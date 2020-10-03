using System.Collections.Generic;

namespace EventHorizon.Identity.AuthServer.Configuration.Initialize.ApiResources
{
    public class ApiResourceFile
    {
        public List<ApiResourceConfiguration> ApiResources { get; set; } = new List<ApiResourceConfiguration>();
    }

    public class ApiResourceConfiguration
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public List<string> ClaimTypes { get; set; }
    }
}
