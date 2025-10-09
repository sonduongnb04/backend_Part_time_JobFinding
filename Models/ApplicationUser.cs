using Microsoft.AspNetCore.Identity;

namespace PTJ.Api.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public bool IsActive { get; set; } = true;
    }
}
