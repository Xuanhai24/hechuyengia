// Models/Disease.cs
using System.ComponentModel.DataAnnotations;

namespace hechuyengia.Models
{
    public class Disease
    {
        public int DiseaseId { get; set; }
        [Required] public string Name { get; set; } = "";
        public string? Description { get; set; }
        public ICollection<DiseaseSymptom> DiseaseSymptoms { get; set; } = new List<DiseaseSymptom>();
    }
}
