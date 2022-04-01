namespace Lab1.Models.Entities.Actions
{
    public class CreateCreditAction : Action
    {
        public double Money { get; set; }
        public double MoneyWithPercent { get; set; }
        public int Percent { get; set; }
        public int Fines { get; set; }
        public int Months { get; set; }
        public int PayedMonths { get; set; }
        public DateTime CreatingTime { get; set; }
        public DateTime PaymentTime { get; set; }
        public string CreditId { get; set; }
        public string BalanceId { get; set; }
        public string BalanceName { get; set; }
    }
}
