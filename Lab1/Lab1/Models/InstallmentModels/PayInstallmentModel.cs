using Lab1.Models.Entities;

namespace Lab1.Models.InstallmentModels
{
    public class PayInstallmentModel
    {
        public List<Balance> Balances { get; set; }
        public string InstallmentId { get; set; }
        public string BalanceId { get; set; }
    }
}
