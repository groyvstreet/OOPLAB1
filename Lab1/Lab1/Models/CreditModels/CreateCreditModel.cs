namespace Lab1.Models.CreditModels
{
    public class CreateCreditModel
    {
        public double Money { get; set; }
        public int Months { get; set; }
        public int Percent { get; set; }
        public string BalanceName { get; set; }
    }
}
