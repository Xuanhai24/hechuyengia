// Controllers/DiagnosesController.cs
using hechuyengia.Data;
using hechuyengia.Models;
using hechuyengia.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hechuyengia.Controllers
{
    [Authorize(Roles = "DOCTOR,ADMIN")]
    [ApiController]
    [Route("api/[controller]")]
    public class DiagnosesController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly PrologService _prolog; // hoặc PrologHelper

        public DiagnosesController(AppDbContext db, PrologService prolog)
        {
            _db = db; _prolog = prolog;
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> ByPatient(int patientId)
        {
            var list = await _db.Diagnoses
                .Where(d => d.PatientId == patientId)
                .Include(d => d.Doctor)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
            return Ok(list);
        }

        public record DiagnoseCreateDto(int PatientId, int DoctorId, List<string> Symptoms);

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DiagnoseCreateDto dto)
        {
            if (!await _db.Patients.AnyAsync(p => p.PatientId == dto.PatientId))
                return NotFound("Patient not found");
            if (!await _db.Doctors.AnyAsync(d => d.DoctorId == dto.DoctorId))
                return NotFound("Doctor not found");

            // gọi Prolog
            var result = _prolog.Diagnose(dto.Symptoms);

            var diag = new Diagnosis
            {
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                SymptomsCsv = string.Join(",", dto.Symptoms),
                Result = result,
                CreatedAt = DateTime.UtcNow
            };
            _db.Diagnoses.Add(diag);
            await _db.SaveChangesAsync();

            var full = await _db.Diagnoses.Include(d => d.Doctor).FirstAsync(d => d.DiagnosisId == diag.DiagnosisId);
            return Ok(full);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var d = await _db.Diagnoses.FindAsync(id);
            if (d == null) return NotFound();
            _db.Remove(d); await _db.SaveChangesAsync(); return Ok();
        }
    }
}
