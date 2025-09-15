// Controllers/DoctorsController.cs
using hechuyengia.Data;
using hechuyengia.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hechuyengia.Controllers
{
    [Authorize(Roles = "ADMIN")]
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public DoctorsController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> Get() =>
            Ok(await _db.Doctors.Include(d => d.User).AsNoTracking().ToListAsync());

        [HttpPost]
        public async Task<IActionResult> Create(Doctor d)
        {
            _db.Doctors.Add(d); await _db.SaveChangesAsync(); return Ok(d);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Doctor d)
        {
            if (id != d.DoctorId) return BadRequest();
            _db.Entry(d).State = EntityState.Modified; await _db.SaveChangesAsync(); return Ok(d);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var x = await _db.Doctors.FindAsync(id);
            if (x == null) return NotFound();
            _db.Remove(x); await _db.SaveChangesAsync(); return Ok();
        }
    }
}
