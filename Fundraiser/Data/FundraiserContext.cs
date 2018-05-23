using Fundraiser.Models;
using Microsoft.EntityFrameworkCore;

namespace Fundraiser.Data
{
    public class FundraiserContext : DbContext
    {
        public FundraiserContext(DbContextOptions<FundraiserContext> options) : base(options)
        {
        }

        public DbSet<Participant> Participants { get; set; }
        public DbSet<Contribution> Contributions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Participant>().ToTable("Participant");
            modelBuilder.Entity<Contribution>().ToTable("Contribution");
        }
    }
}
