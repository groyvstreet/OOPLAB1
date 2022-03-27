using Lab1.Models.Entities;

namespace Lab1.Models.CreditModels
{
    public class PayCreditModel
    {
        public List<Balance> Balances { get; set; }
        public string CreditId { get; set; }
        public string BalanceId { get; set; }
    }
}
