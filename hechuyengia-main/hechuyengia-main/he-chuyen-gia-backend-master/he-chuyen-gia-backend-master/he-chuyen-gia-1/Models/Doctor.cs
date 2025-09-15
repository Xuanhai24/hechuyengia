// Models/Doctor.cs
using System.ComponentModel.DataAnnotations;

namespace hechuyengia.Models
{
    public class Doctor
    {
        public int DoctorId { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; } = "";
        public string? Specialty { get; set; }
        public ICollection<Diagnosis> Diagnoses { get; set; } = new List<Diagnosis>();
    }
}
