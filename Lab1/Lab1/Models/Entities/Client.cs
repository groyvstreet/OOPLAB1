namespace Lab1.Models.Entities
{
    public class Client : User
    {
        public string? PassportSeries { get; set; }
        public int PassportNumber { get; set; }
        public string? IdentificationNumber { get; set; }
        public List<Deposit> Deposits { get; set; } = new List<Deposit>();
        public List<Balance> Balances { get; set; } = new List<Balance>();
        public List<Credit> Credits { get; set; } = new List<Credit>();
        public int Percent { get; set; }
        public List<Installment> Installments { get; set; } = new List<Installment>();
        public bool Approved { get; set; }
        public string CompanyId { get; set; }
        public Salary Salary { get; set; }
    }
}
