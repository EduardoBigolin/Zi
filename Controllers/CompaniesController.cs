using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zi.Data;
using Zi.Models;
using Zi.Services;

namespace Zi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly DataContext _context;

        public CompaniesController(DataContext context)
        {
            _context = context;
        }

        // POST: api/Companies/login
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _context.Company.FirstOrDefaultAsync(c => c.Email == request.Email);

                if (user == null)
                {
                    return NotFound(new { error = "Company not found" });
                }
                var validPassword = HashPassword.VerifyPassword(request.Password, user.Password);
                if (!validPassword)
                {
                    return BadRequest(new { error = "Invalid password" });
                };

                return Ok(new { token = Auth.GenerateToken(user.Name, "company", user.Id.ToString()) });

            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex });
            }
        }

        // GET: api/Companies/
        [HttpGet]
        [Authorize(Roles = "company")]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
            var companyIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            int companyId;

            if (!string.IsNullOrEmpty(companyIdClaim) && int.TryParse(companyIdClaim, out companyId))
            {
                var company = await _context.Company.FindAsync(companyId);

                if (company == null)
                {
                    return NotFound();
                }

                return company;
            }

            return BadRequest(new { error = "Invalid id" });

        }

        [HttpGet("employees")]
        [Authorize(Roles = "company")]
        public async Task<IActionResult> GetEmployees()
        {
            try
            {
                var companyIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
                int companyId;

                if (!string.IsNullOrEmpty(companyIdClaim) && int.TryParse(companyIdClaim, out companyId))
                {
                    var Employees = await _context.Employees.Where(e => e.CompanyId == companyId).ToListAsync();

                    return Ok(Employees);
                }
                else
                {
                    return BadRequest(new { error = "Invalid id" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Companies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompany(int id, Company company)
        {
            if (id != company.Id)
            {
                return BadRequest();
            }

            _context.Entry(company).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Companies
        [HttpPost]
        public async Task<ActionResult<Company>> PostCompany(Company company)
        {
            var existingCompany = await _context.Company.FirstOrDefaultAsync(c => c.Email == company.Email);
            if (existingCompany != null)
            {
                return Conflict(new { error = "This email is already in use" });
            }

            var passwordHash = HashPassword.CreateHashPassword(company.Password);

            company.Password = passwordHash;

            _ = _context.Company.Add(company);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCompany", new { id = company.Id }, company);
        }

        // DELETE: api/Companies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var company = await _context.Company.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            _context.Company.Remove(company);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool CompanyExists(int id)
        {
            return _context.Company.Any(e => e.Id == id);
        }
    }
}
