using Helios.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Helios.Data
{
    public class HeliosDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Entity> Entities {get; set;}
        public DbSet<Trait> Traits {get; set;}
        public DbSet<Account_Entity> Characters {get; set;}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            //optionsBuilder.UseSqlServer("Data Source=(localdb)\\ProjectsV13;Initial Catalog=AADb;Integrated Security=True;Persist Security Info=False");
            optionsBuilder.UseNpgsql("Server=127.0.0.1;Port=5432;Database=HeliosDb;User Id=postgres;Password=xxxxx;");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<User>()
               .HasOne(p => p.Account)
               .WithOne(i => i.User)
               .HasForeignKey<Account>(a => a.UserId);

            builder.Entity<Account>()
                .HasMany(x => x.Characters)
                .WithOne(c => c.Account)
                .HasForeignKey(c => c.AccountId);

            builder.Entity<Entity>()
                .HasMany(x => x.Components)
                .WithOne(x => x.Entity)
                .HasForeignKey(x => x.EntityId);

            builder.Entity<Account_Entity>()
                .HasKey(x => new { x.AccountId, x.EntityId });
        }
    }
}
