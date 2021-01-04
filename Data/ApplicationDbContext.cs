using Microsoft.EntityFrameworkCore;
using NationalParkAPI.Models;

namespace NationalParkAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<NationalPark> NationalParks { get; set; }
    }
}