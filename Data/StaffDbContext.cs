using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Data
{
    public class StaffDbContext : DbContext
	{
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<StaffTitle> StaffTitles { get; set; }
        public DbSet<Log> Logs { get; set; }

        public StaffDbContext(DbContextOptions options) : base(options)
		{
        }

        protected override void OnModelCreating(ModelBuilder builder)
		{
            base.OnModelCreating(builder);
        }
    }
}