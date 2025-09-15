﻿// Controllers/AuthController.cs
using System.Security.Claims;
using hechuyengia.Data;
using hechuyengia.Models;
using hechuyengia.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hechuyengia.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IJwtService _jwt;

        public AuthController(AppDbContext db, IJwtService jwt)
        {
            _db = db; _jwt = jwt;
        }

        public record LoginDto(string Email, string Password);
        public record RegisterDto(string Email, string FullName, string Role, string Password);

        [HttpPost("register")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (await _db.Users.AnyAsync(x => x.Email == dto.Email))
                return BadRequest("Email already exists");

            var u = new User
            {
                Email = dto.Email,
                FullName = dto.FullName,
                Role = dto.Role, // "ADMIN" | "DOCTOR"
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _db.Users.Add(u);
            await _db.SaveChangesAsync();

            return Ok(new { u.UserId, u.Email, u.FullName, u.Role });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var u = await _db.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);
            if (u == null || !BCrypt.Net.BCrypt.Verify(dto.Password, u.PasswordHash))
                return Unauthorized("Invalid credentials");

            var token = _jwt.CreateToken(u);
            return Ok(new { token, user = new { u.UserId, u.Email, u.FullName, u.Role } });
        }
        // đặt cạnh Login/Me
        [AllowAnonymous]
        [HttpPost("register-doctor")]
        public async Task<IActionResult> RegisterDoctor([FromBody] RegisterDoctorDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.FullName) ||
                string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Thiếu thông tin");

            var email = dto.Email.Trim();
            if (await _db.Users.AnyAsync(x => x.Email == email))
                return BadRequest("Email đã tồn tại");

            var u = new User
            {
                Email = email,
                FullName = dto.FullName.Trim(),
                Role = "DOCTOR", // cố định là bác sĩ
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _db.Users.Add(u);
            await _db.SaveChangesAsync();

            // tự đăng nhập luôn sau khi đăng ký
            var token = _jwt.CreateToken(u);
            return Ok(new { token, user = new { u.UserId, u.Email, u.FullName, u.Role } });
        }

        public record RegisterDoctorDto(string Email, string FullName, string Password);

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            // Ưu tiên lấy theo NameIdentifier/Sub (UserId) để chắc chắn
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? User.FindFirstValue("sub");
            if (!int.TryParse(idClaim, out var uid)) return Unauthorized();

            var u = await _db.Users.FirstOrDefaultAsync(x => x.UserId == uid);
            if (u == null) return NotFound();

            return Ok(new { u.UserId, u.Email, u.FullName, u.Role });
        }
    }
}
