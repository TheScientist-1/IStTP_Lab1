
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GalleryWebApplication.Models;

namespace GalleryWebApplication.Models
{
    public class IdentityContext : IdentityDbContext<User>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options)
                : base(options)
        {
            Database.EnsureCreated();
        }

    }
}
