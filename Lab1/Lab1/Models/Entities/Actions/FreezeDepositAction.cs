namespace Lab1.Models.Entities.Actions
{
    public class FreezeDepositAction : DepositAction
    {
        public string ClientEmail { get; set; }
        public bool Closed { get; set; }
    }
}
