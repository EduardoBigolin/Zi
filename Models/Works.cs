namespace Zi.Models
{
    public class Works
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public ICollection<WorkEmployee> WorkEmployees { get; set; }
    }
}
