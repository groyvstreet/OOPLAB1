using System.ComponentModel.DataAnnotations;

namespace Lab1.Models.Entities
{
    public class Company
    {
        [Key] public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Type { get; set; }
        public string? LegalName { get; set; }
        public string? LegalAddress { get; set; }
        public string? PIN { get; set; }
        public string? BIC { get; set; }
        public double SalaryMoney { get; set; }
        public int Percent { get; set; }
        public string? BankId { get; set; }
    }
}
