using Elysian.Domain.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Elysian.Infrastructure.Context
{
    public partial class ElysianContext(DbContextOptions<ElysianContext> options) : DbContext(options)
    {
        public DbSet<Card> Cards { get; set; }
        public DbSet<CardImage> CardImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ElysianContext).Assembly);
        }
    }

    public class ElysianContextFactory : IDesignTimeDbContextFactory<ElysianContext>
    {
        public ElysianContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ElysianContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Elysian;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new ElysianContext(optionsBuilder.Options);
        }
    }
}
