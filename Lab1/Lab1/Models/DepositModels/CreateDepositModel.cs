using System.ComponentModel.DataAnnotations;

namespace Lab1.Models.DepositModels
{
    public class CreateDepositModel
    {
        [Required(ErrorMessage = "Не указано количество денег")]
        public double Money { get; set; }

        [Required(ErrorMessage = "Не указан процент")]
        public int Percent { get; set; }

        public DateTime ClosedTime { get; set; }
        public string? BalanceName { get; set; }
        public string? ClientId { get; set; }
    }
}
