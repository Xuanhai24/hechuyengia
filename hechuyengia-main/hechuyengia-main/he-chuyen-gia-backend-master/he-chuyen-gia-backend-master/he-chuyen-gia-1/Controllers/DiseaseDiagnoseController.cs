using hechuyengia.Data;
using hechuyengia.Services;
using hechuyengia.Models.DiseaseDiagnose;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hechuyengia.Controllers
{
    [Route("api/chuan-doan-benh")]
    [ApiController]
    public class DiseaseDiagnoseController : ControllerBase
    {
        private static readonly object _lock = new object();
        private ILogger<DiseaseDiagnoseController> _logger;

        private PrologService _prolog = new PrologService();
        private AppDbContext _db;
        public DiseaseDiagnoseController(AppDbContext db, ILogger<DiseaseDiagnoseController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet("danh-sach-trieu-chung")]
        public ActionResult<List<string>> GetSystompList()
        {
            lock (_lock)
            {
                var res = _prolog.GetSymptoms();
                return Ok(res);
            }
        }
        [HttpPost("chuan-doan")]
        public IActionResult GetDisease([FromBody] string[] symptoms)
        {
            //input la 1 mang trieu chung
            lock (_lock)
            {
                if (symptoms == null || symptoms.Length == 0)
                {
                    return BadRequest("Danh sách triệu chứng không được để trống.");
                }
                var result = _prolog.GetDiseases(symptoms);
                return Ok(result);
            }
        }
        [HttpPost("luu-ket-qua")]
        public IActionResult saveDiagnosedResult([FromBody] DiagnoseResult ds)
        {
            _logger.LogInformation(ds.PatientId.ToString());
            _db.diagnoseResults.Add(ds);
            _db.SaveChanges();
            return NoContent();
        }
    }
}
