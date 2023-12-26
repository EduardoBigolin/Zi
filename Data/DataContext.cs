using Microsoft.EntityFrameworkCore;
using Zi.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Zi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Company> Company => Set<Company>();
        public DbSet<Employees> Employees => Set<Employees>();
        public DbSet<WorkEmployee> WorkEmployees => Set<WorkEmployee>();
        public DbSet<Works> Works => Set<Works>();
    }
}
