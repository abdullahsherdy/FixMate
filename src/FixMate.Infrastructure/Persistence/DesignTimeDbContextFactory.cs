using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace FixMate.Infrastructure.Persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FixMateDbContext>
    {
        public FixMateDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<FixMateDbContext>();
            var connectionString = "Server=.;Database=FixMateDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
            
            builder.UseSqlServer(connectionString);

            return new FixMateDbContext(builder.Options);
        }
    }
} 