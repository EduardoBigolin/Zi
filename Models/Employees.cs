using System.ComponentModel.DataAnnotations;

namespace Zi.Models
{
    public class Employees
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone is required")]
        public string Phone { get; set; } = string.Empty;

        public int CompanyId { get; set; }

        public Company Company { get; set; }

        public ICollection<WorkEmployee> WorkEmployees { get; set; }
    }
}
