using Microsoft.EntityFrameworkCore;
using projekt_api.Models;

namespace projekt_api.Data
{
    public class ApiContext : DbContext
    {
        public DbSet<Devices> Devices { get; set; } = null!;

        public ApiContext(DbContextOptions<ApiContext> options)
        : base(options)
        {
        }
    }
}