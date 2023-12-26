using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Zi.Models.DTOs.EmployeesDto;
using Zi.Data;
using Zi.Models;
using Zi.Services;
using Microsoft.AspNetCore.Identity.Data;

namespace Zi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly DataContext _context;

        public EmployeesController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employees>>> GetEmployees()
        {
            return await _context.Employees.ToListAsync();
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employees>> GetEmployees(int id)
        {
            var employees = await _context.Employees.FindAsync(id);

            if (employees == null)
            {
                return NotFound();
            }

            return employees;
        }

        // PUT: api/Employees/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployees(int id, Employees employees)
        {
            if (id != employees.id)
            {
                return BadRequest();
            }

            _context.Entry(employees).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeesExists(id))
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

        // POST: api/Employees/login
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginRequest request)
        {
            var employees = await _context.Employees.FirstOrDefaultAsync(c => c.Email == request.Email);

            if (employees == null)
            {
                return NotFound(new { error = "Account not found" });
            }
            var validPassword = HashPassword.VerifyPassword(request.Password, employees.Password);
            if (!validPassword)
            {
                return BadRequest(new { error = "Invalid password" });
            };

            return Ok(new { token = Auth.GenerateToken(employees.Name, "employees", employees.id.ToString()) });
        }

        // POST: api/Employees
        [HttpPost]
        [Authorize(Roles = "company")]
        public async Task<ActionResult<Employees>> PostEmployees(EmployeesDTORequest employees)
        {
            var companyIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            int companyId;

            if (!string.IsNullOrEmpty(companyIdClaim) && int.TryParse(companyIdClaim, out companyId))
            {
                var existCompany = await _context.Company.FirstOrDefaultAsync(c => c.Id == companyId);

                if (existCompany == null)
                {
                    return NotFound("Company not found");
                }

                var existEmployee = await _context.Employees.FirstOrDefaultAsync(c => c.Email == employees.Email);
                if (existEmployee != null)
                {
                    return BadRequest("This email alredy in use");
                }

                var newEmployees = new Employees();
                newEmployees.Name = employees.Name;
                newEmployees.Email = employees.Email;
                newEmployees.Password = HashPassword.CreateHashPassword(employees.Password);
                newEmployees.Phone = employees.Phone;
                newEmployees.CompanyId = existCompany.Id;
                newEmployees.Company = existCompany;

                _context.Employees.Add(newEmployees);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Employ Created with sucess", newEmployees });
            }
            else
            {
                return BadRequest("Invalid number");
            }
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployees(int id)
        {
            var employees = await _context.Employees.FindAsync(id);
            if (employees == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employees);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeesExists(int id)
        {
            return _context.Employees.Any(e => e.id == id);
        }
    }
}
