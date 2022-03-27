using System.ComponentModel.DataAnnotations;

namespace Lab1.Models.Entities
{
    public class CreditApproving
    {
        [Key] public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CreditId { get; set; }
        public string BalanceId { get; set; }
    }
}
