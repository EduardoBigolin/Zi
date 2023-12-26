using static Zi.Models.DTOs.EmployeesDto;

namespace Zi.Models.DTOs
{
    public class WorksDto
    {
        public class WorksDTOResponse
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public ICollection<EmployeeDTOResponse> Employees { get; set; } = new List<EmployeeDTOResponse>();
        }

        public class WorksRequest
        {
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
        };
    }
}
