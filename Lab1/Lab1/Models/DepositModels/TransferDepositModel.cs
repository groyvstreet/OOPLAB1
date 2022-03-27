using Lab1.Models.Entities;

namespace Lab1.Models.DepositModels
{
    public class TransferDepositModel
    {
        public List<Deposit> Deposits { get; set; }
        public DateTime NowTime { get; set; }
        public string IdFrom { get; set; }
        public string IdTo { get; set; }
        public double Money { get; set; }
    }
}
