using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RESTApi.Models
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        /// <summary>
        /// Entities
        /// </summary>
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<TokenInfo> TokenInfos { get; set; }
    }
}
