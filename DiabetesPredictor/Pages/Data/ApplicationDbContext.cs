using DiabetesPredictor.Pages.Models;
using Microsoft.EntityFrameworkCore;

namespace DiabetesPredictor.Pages.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Patient> Patients { get; set; }
    }
}