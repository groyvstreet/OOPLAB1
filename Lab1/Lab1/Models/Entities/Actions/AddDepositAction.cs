namespace Lab1.Models.Entities.Actions
{
    public class AddDepositAction : DepositAction
    {
        public double AddedMoney { get; set; }
        public bool Closed { get; set; }
    }
}
