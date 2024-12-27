using BBMS_WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BBMS_WebAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<OtpModel> Otps { get; set; }
    }
}
