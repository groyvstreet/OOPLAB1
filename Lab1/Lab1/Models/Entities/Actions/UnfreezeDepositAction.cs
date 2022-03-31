namespace Lab1.Models.Entities.Actions
{
    public class UnfreezeDepositAction : DepositAction
    {
        public string ClientEmail { get; set; }
        public bool Closed { get; set; }
    }
}
