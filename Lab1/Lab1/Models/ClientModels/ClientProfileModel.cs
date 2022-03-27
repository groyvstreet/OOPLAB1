using Lab1.Models.Entities;

namespace Lab1.Models.ClientModels
{
    public class ClientProfileModel
    {
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Patronymic { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PassportSeries { get; set; }
        public int PassportNumber { get; set; }
        public string? IdentificationNumber { get; set; }
        public List<Deposit> Deposits { get; set; }
        public DateTime NowTime { get; set; }
        public List<Balance> Balances { get; set; }
        public List<Credit> Credits { get; set; }
        public List<Installment> Installments { get; set; }
        public bool Approved { get; set; }
        public Salary Salary { get; set; }
        public int Percent { get; set; }
        public string CompanyName { get; set; }
        public string BankName { get; set; }
    }
}
