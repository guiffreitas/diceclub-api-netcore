using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using diceclub_api_netcore.Domain.Entities;

namespace diceclub_api_netcore.Infrastructure.Contexts
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<UserProfile> Profiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(b =>
            {
                b.ToTable("user");
            });

            modelBuilder.Entity<IdentityUserClaim<int>>(b =>
            {
                b.ToTable("user_claim");
            });

            modelBuilder.Entity<IdentityUserLogin<int>>(b =>
            {
                b.ToTable("user_login");
            });

            modelBuilder.Entity<IdentityUserToken<int>>(b =>
            {
                b.ToTable("user_token");
            });

            modelBuilder.Entity<IdentityRole<int>>(b =>
            {
                b.ToTable("role");
            });

            modelBuilder.Entity<IdentityRoleClaim<int>>(b =>
            {
                b.ToTable("role_claim");
            });

            modelBuilder.Entity<IdentityUserRole<int>>(b =>
            {
                b.ToTable("user_role");
            });

            modelBuilder.Entity<UserProfile>()
                .ToTable("user_profile")
                .HasOne(e => e.User)
                .WithOne(e => e.Profile)
                .HasForeignKey<UserProfile>(e => e.UserId)
                .IsRequired();
        }
    }
}
