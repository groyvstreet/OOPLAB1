namespace Lab1.Models.Entities.Actions
{
    public class AddBalanceAction : Action
    {
        public string BalanceId { get; set; }
        public string BalanceName { get; set; }
        public double Money { get; set; }
    }
}
