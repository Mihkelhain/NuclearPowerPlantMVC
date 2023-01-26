using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NuclearPowerPlantMVC.Models;

namespace NuclearPowerPlantMVC.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<NuclearPlant> NuclearPlants { get; set; }
        public DbSet<Reactor> Reactors { get; set; }
    }
}