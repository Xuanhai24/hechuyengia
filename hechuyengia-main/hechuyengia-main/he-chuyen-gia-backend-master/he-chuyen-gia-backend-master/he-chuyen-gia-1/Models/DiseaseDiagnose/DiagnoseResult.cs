using System.ComponentModel.DataAnnotations;

namespace hechuyengia.Models.DiseaseDiagnose
{
    public class DiagnoseResult
    {
        [Key]
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }
        public string Symptoms { get; set; }
        public string Diseases { get; set; }
        public string MedicinesAdvice { get; set; }
        public string DoctorAdvice { get; set; }
        public string DiagnoseDate { get; set; }
        public string DoctorName { get; set; }
    }
}
