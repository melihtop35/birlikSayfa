using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace sayfaASP.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UsersInfo> UsersInfo { get; set; }
        public DbSet<UsersUnit> UsersUnit { get; set; }
    }
}
