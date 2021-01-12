using System.Diagnostics;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;

namespace EventHorizon.Identity.AuthServer.StatusCode
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class IndexModel
        : PageModel
    {
        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public int ErrorStatusCode { get; set; }
        public string ErrorMessage { get; set; }

        public void OnGet(int code)
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            ErrorStatusCode = code;

            if (ErrorStatusCode == 404)
            {
                ErrorMessage = "The requested page not found.";
            }
            else if (ErrorStatusCode == 500)
            {
                ErrorMessage = "My custom 500 error message.";
            }
            else
            {
                ErrorMessage = "An error occurred while processing your request.";
            }
        }
    }
}