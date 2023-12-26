namespace Zi.Models
{
    public class WorkEmployee
    {
        public int Id { get; set; }
        public int WorkId { get; set; }
        public Works Work { get; set; }
        public string Lavel { get; set; }
        public int EmployeeId { get; set; }
        public Employees Employee { get; set; }
    }
}
