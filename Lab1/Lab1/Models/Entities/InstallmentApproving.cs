using System.ComponentModel.DataAnnotations;

namespace Lab1.Models.Entities
{
    public class InstallmentApproving
    {
        [Key] public string Id { get; set; } = Guid.NewGuid().ToString();
        public string InstallmentId { get; set; }
        public string BalanceId { get; set; }
    }
}
