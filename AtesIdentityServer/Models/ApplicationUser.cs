using Microsoft.AspNetCore.Identity;

namespace AtesIdentityServer.Models
{
    public class ApplicationUser : IdentityUser
    {
        public Guid PublicId { get; set; } = Guid.NewGuid();
        public string Role { get; set; } = "dev";
    }
}
