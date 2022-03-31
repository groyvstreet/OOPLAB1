namespace Lab1.Models.Entities.Actions
{
    public class DepositAction : Action
    {
        public string DepositId { get; set; }
        public double Money { get; set; }
        public int Percent { get; set; }
        public DateTime OpenedTime { get; set; }
        public DateTime ClosedTime { get; set; }
    }
}
