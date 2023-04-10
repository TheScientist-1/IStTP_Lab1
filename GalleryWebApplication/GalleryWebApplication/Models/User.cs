using Microsoft.AspNetCore.Identity;

namespace GalleryWebApplication.Models

{
    public class User: IdentityUser
    {
        public string Name { get; set; }
    }
}
