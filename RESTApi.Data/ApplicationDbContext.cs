using Microsoft.EntityFrameworkCore;
using RESTApi.Data.Models;

namespace RESTApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        /// <summary>
        /// Entities
        /// </summary>
        public DbSet<Doctor> Doctors { get; set; }

        public virtual async Task<int> SaveChangesAsync()
        {
            OnBeforeSaveChanges();
            return await base.SaveChangesAsync();
        }

        private void OnBeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();
            foreach (var entity in ChangeTracker
                .Entries()
                .Where(x => x.Entity is BaseModel && x.State == EntityState.Modified)
                .Select(x => x.Entity)
                .Cast<BaseModel>())
            {
                entity.UpdatedDate = DateTime.Now;
            }

            foreach (var entity in ChangeTracker
                .Entries()
                .Where(x => x.Entity is BaseModel && x.State == EntityState.Added)
                .Select(x => x.Entity)
                .Cast<BaseModel>())
            {
                entity.CreatedDate = DateTime.Now;
            }
        }
    }
}
