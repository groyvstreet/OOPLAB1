using Lab1.Models.Entities;

namespace Lab1.Models.SpecialistModels
{
    public class SpecialistProfileModel
    {
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Patronymic { get; set; }
        public string? PhoneNumber { get; set; }
        public List<Balance> Balances { get; set; }
        public string CompanyName { get; set; }
        public string BankName { get; set; }
    }
}
