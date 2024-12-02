using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using VehicleRentalApp.Models;

namespace VehicleRentalApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Vehicle> Vehicles { get; set; }  // Vehicle modeline karşılık gelen DbSet
        public DbSet<User> Users { get; set; }
    }
}
