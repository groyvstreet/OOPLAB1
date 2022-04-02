using System.ComponentModel.DataAnnotations;

namespace Lab1.Models.DepositModels
{
    public class CreateDepositModel
    {
        [Required(ErrorMessage = "Не указана сумма")]
        [RegularExpression(@"[0](?:[.][0-9][1-9]|[.][1-9])?|[1-9]+[0-9]{0,9}(?:[.][0-9][1-9]|[.][1-9])?", ErrorMessage = "Некорректный ввод")]
        public string Money { get; set; }

        [Required(ErrorMessage = "Не указан процент")]
        [RegularExpression(@"(?:[1][0][0])|(?:[1-9][0-9]?)", ErrorMessage = "Некорректный ввод (от 1 до 100)")]
        public int Percent { get; set; }

        public DateTime ClosedTime { get; set; }
        public string? BalanceName { get; set; }
        public string? ClientId { get; set; }
    }
}
