namespace Lab1.Models.Entities.Actions
{
    public class PayInstallmentAction : Action
    {
        public string InstallmentId { get; set; }
        public string BalanceId { get; set; }
        public string BalanceName { get; set; }
        public double Money { get; set; }
        public double PayMoney { get; set; }
        public int Months { get; set; }
        public int PayedMonths { get; set; }
        public DateTime CreatingTime { get; set; }
        public DateTime PaymentTime { get; set; }
        public double SinglePaymentMoney { get; set; }
    }
}
