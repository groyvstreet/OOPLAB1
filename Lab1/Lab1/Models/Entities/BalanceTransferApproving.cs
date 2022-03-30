using System.ComponentModel.DataAnnotations;

namespace Lab1.Models.Entities
{
    public class BalanceTransferApproving
    {
        [Key] public string Id { get; set; } = Guid.NewGuid().ToString();
        public double Money { get; set; }
        public string BalanceIdFrom { get; set; }
        public string BalanceIdTo { get; set; }
    }
}
