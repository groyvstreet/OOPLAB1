using Lab1.Models.Entities;

namespace Lab1.Models.DepositModels
{
    public class GetDepositModel
    {
        public double Money { get; set; }
        public List<Balance> Balances { get; set; }
        public string DepositId { get; set; }
        public string BalanceId { get; set; }
    }
}
