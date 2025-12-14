using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicWebsite.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string DoctorName { get; set; }
    }
}
