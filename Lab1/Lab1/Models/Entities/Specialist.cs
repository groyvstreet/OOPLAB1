namespace Lab1.Models.Entities
{
    public class Specialist : User
    {
        public List<Balance> Balances { get; set; } = new List<Balance>();
        public string CompanyId { get; set; }
    }
}
