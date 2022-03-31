namespace Lab1.Models.Entities.Actions
{
    public class GetDepositAction : DepositAction
    {
        public string BalanceId { get; set; }
        public double MoneyWithPercent { get; set; }
    }
}
