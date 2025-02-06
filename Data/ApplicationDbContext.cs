using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data
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
        public DbSet<TokenInfo> Tokens { get; set; }
    }
}
