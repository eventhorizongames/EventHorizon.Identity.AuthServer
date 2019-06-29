using System.ComponentModel.DataAnnotations;

namespace EventHorizon.Identity.AuthServer.Clients.Models
{
    public class ClientCreateModel
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}