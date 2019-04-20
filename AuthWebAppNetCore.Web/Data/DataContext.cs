using AuthWebAppNetCore.Web.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AuthWebAppNetCore.Web.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }

        // If you want personalize a model in DB, it has to be here
        protected override void OnModelCreating(ModelBuilder builder)
        {
           // builder.Entity<Product>()
           //.Property(p => p.Price)
           //.HasColumnType("decimal(18,2)");

            // If you delete a user, it can not delete a City or other models in cascade
            var cascadeFKs = builder.Model
                .G­etEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Casca­de);
            foreach (var fk in cascadeFKs)
            {
                fk.DeleteBehavior = DeleteBehavior.Restr­ict;
            }

            base.OnModelCreating(builder);
        }
    }
}
