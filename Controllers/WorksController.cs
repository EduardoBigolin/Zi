using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Zi.Models.DTOs.EmployeesDto;
using static Zi.Models.DTOs.EmployeesWorksDto;
using static Zi.Models.DTOs.WorksDto;
using Zi.Data;
using Zi.Models;

namespace Zi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorksController : ControllerBase
    {
        private readonly DataContext _context;

        public WorksController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Works
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Works>>> GetWorks()
        {
            return await _context.Works.ToListAsync();
        }

        // GET: api/Works/5
        [HttpGet("{id}")]
        [Authorize(Roles = "company")]
        public async Task<ActionResult<WorksDTOResponse>> GetMyWork(int id)
        {
            var work = await _context.Works
                .Include(w => w.WorkEmployees)
                .ThenInclude(we => we.Employee)
                .FirstOrDefaultAsync(w => w.id == id);

            if (work == null)
            {
                return NotFound();
            }

            var worksDTO = new WorksDTOResponse
            {
                Id = work.id,
                Name = work.name,
                Description = work.description,
                Employees = work.WorkEmployees.Select(we => new EmployeeDTOResponse
                {
                    Id = we.Employee.id,
                    Name = we.Employee.Name,
                    Email = we.Employee.Email,
                    Phone = we.Employee.Phone
                }).ToList()
            };

            return worksDTO;
        }

        // PUT: api/Works/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWorks(int id, Works works)
        {
            if (id != works.id)
            {
                return BadRequest();
            }

            _context.Entry(works).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorksExists(id))
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

        // POST: api/Works/add/1
        [HttpPost("addEmployees/{id}")]
        public async Task<IActionResult> AddEmployeesWork(int id, EmployeesWorkRequest request)
        {
            var existEmployee = await _context.Employees.FirstOrDefaultAsync(c => c.Email == request.Email); ;

            if (existEmployee == null)
            {
                return NotFound("User not found");
            }

            var isEmployeeAssigned = await _context.WorkEmployees
                .AnyAsync(we => we.WorkId == id && we.EmployeeId == existEmployee.id);

            if (isEmployeeAssigned)
            {
                return BadRequest($"Employee with email {request.Email} is already assigned to this work");
            }

            var newWorkEmployee = new WorkEmployee
            {
                WorkId = id,
                EmployeeId = existEmployee.id
            };

            _context.WorkEmployees.Add(newWorkEmployee);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Employee added to work successfully", request.Email });
        }

        // POST: api/Works
        [HttpPost]
        [Authorize(Roles = "company")]
        public async Task<ActionResult<Works>> PostWorks(WorksRequest works)
        {
            var companyIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

            if (companyIdClaim == null)
            {
                return BadRequest("Invalid company");
            }

            var companyId = int.Parse(companyIdClaim);
            var newWork = new Works() { name = works.Name, description = works.Description };
            newWork.CompanyId = companyId;

            var companyExist = await _context.Company.FirstOrDefaultAsync(c => c.Id == companyId);
            if (companyExist == null)
            {
                return BadRequest("Invalid Company");
            }
            newWork.Company = companyExist;

            _context.Works.Add(newWork);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWorks", new { id = newWork.id }, newWork);
        }

        // DELETE: api/Works/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorks(int id)
        {
            var works = await _context.Works.FindAsync(id);
            if (works == null)
            {
                return NotFound();
            }

            _context.Works.Remove(works);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WorksExists(int id)
        {
            return _context.Works.Any(e => e.id == id);
        }
    }
}
