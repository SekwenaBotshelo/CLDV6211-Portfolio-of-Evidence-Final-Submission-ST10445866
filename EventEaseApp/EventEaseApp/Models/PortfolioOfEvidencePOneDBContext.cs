using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace EventEaseApp.Models
{
    public partial class PortfolioOfEvidencePOneDBContext : DbContext
    {
        public PortfolioOfEvidencePOneDBContext()
            : base("name=PortfolioOfEvidencePOneDBContext")
        {
        }

        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Venue> Venues { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>()
                .HasMany(e => e.Bookings)
                .WithRequired(e => e.Event)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Venue>()
                .HasMany(e => e.Bookings)
                .WithRequired(e => e.Venue)
                .WillCascadeOnDelete(false);
        }
    }
}
