using Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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
        }
   
    }
}
