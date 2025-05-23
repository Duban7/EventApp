using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Data.Context
{
    public class EventAppDbContext : IdentityDbContext<User>
    {
        public DbSet<Event> Events { get; set; }
        public DbSet<EventParticipation> EventParticipations { get; set; }
        public EventAppDbContext(DbContextOptions<EventAppDbContext> options) 
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Event>()
                .HasIndex(e => e.Name).IsUnique();

            builder.Entity<Event>()
                .Property(e => e.Name).IsRequired();
            builder.Entity<Event>()
                .Property(e => e.StartDate).IsRequired();


            builder.Entity<Event>()
                .HasMany(e => e.Participants)
                .WithMany(p => p.Events)
                .UsingEntity<EventParticipation>(
                    ep=>ep.Property(ep=>ep.RegistrationDate).HasDefaultValueSql("CURRENT_TIMESTAMP"));

            base.OnModelCreating(builder);

            AddAdminUser(builder);
        }

        private void AddAdminUser(ModelBuilder builder)
        {
            const string adminRoleId = "1337";
            builder.Entity<IdentityRole>().HasData(
               new IdentityRole
               {
                   Id = adminRoleId,
                   Name = "Admin",
                   NormalizedName = "ADMIN"
               });

            const string adminUserId = "1337";
            var adminUser = new IdentityUser
            {
                Id = adminUserId,
                UserName = "admin@mail.com",
                Email = "admin@mail.com",
                NormalizedUserName = "ADMIN@MAIL.COM",
                NormalizedEmail = "ADMIN@MAIL.COM",
                EmailConfirmed = true
            };

            // Хешируем пароль
            var hasher = new PasswordHasher<IdentityUser>();
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "admin1337");

            builder.Entity<IdentityUser>().HasData(adminUser);

            // 3. Назначаем роль пользователю
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = adminRoleId,
                    UserId = adminUserId
                });
        }
    }
}
