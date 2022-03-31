namespace Lab1.Models.Entities.Actions
{
    public class TransferDepositAction : DepositAction
    {
        public double TransferMoney { get; set; }
        public string DepositIdTo { get; set; }
        public bool DepositFromClosed { get; set; }
        public bool DepositToClosed { get; set; }
        public double DepositToMoney { get; set; }
        public int DepositToPercent { get; set; }
        public DateTime DepositToOpenedTime { get; set; }
        public DateTime DepositToClosedTime { get; set; }
    }
}
