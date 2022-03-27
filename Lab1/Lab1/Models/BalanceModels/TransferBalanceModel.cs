using Lab1.Models.Entities;

namespace Lab1.Models.BalanceModels
{
    public class TransferBalanceModel
    {
        public List<Balance> Balances { get; set; }
        public string IdFrom { get; set; }
        public double Money { get; set; }
        public string EmailTo { get; set; }
        public string BankNameTo { get; set; }
        public string BalanceNameTo { get; set; }
    }
}
