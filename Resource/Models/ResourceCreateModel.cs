using System.ComponentModel.DataAnnotations;

namespace EventHorizon.Identity.AuthServer.Resource.Models
{
    public class ResourceCreateModel
    {
        public string ReturnClient { get; set; }
        [Display(Name = "Name", Prompt = "Name")]
        public string Name { get; set; }
        [Display(Name = "Display Name", Prompt = "Display Name")]
        public string DisplayName { get; set; }
    }
}