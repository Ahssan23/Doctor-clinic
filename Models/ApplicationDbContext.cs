using Microsoft.EntityFrameworkCore;

namespace ClinicWebsite.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        // Add this line to include Appointments table
        public DbSet<Appointment> Appointments { get; set; }
    }
}
