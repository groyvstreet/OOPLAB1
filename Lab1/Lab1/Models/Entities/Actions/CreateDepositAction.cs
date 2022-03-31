namespace Lab1.Models.Entities.Actions
{
    public class CreateDepositAction : DepositAction 
    {
        public string? BalanceId { get; set; }
        public bool Closed { get; set; }
    }
}
