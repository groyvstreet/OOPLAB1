namespace Lab1.Models.Entities.Actions
{
    public class CloseBalanceAction : Action
    {
        public double Money { get; set; }
        public string BalanceId { get; set; }
        public string BalanceName { get; set; }
    }
}
