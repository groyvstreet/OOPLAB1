using System.ComponentModel.DataAnnotations;

namespace Lab1.Models.InstallmentModels
{
    public class CreateInstallmentModel
    {
        [Required(ErrorMessage = "Не указана сумма")]
        [RegularExpression(@"[0](?:[.][0-9][1-9]|[.][1-9])?|[1-9]+[0-9]{0,9}(?:[.][0-9][1-9]|[.][1-9])?", ErrorMessage = "Некорректный ввод")]
        public string Money { get; set; }

        [Required(ErrorMessage = "Не указано количество месяцев")]
        [RegularExpression(@"[1-9][0-9]*[0-9]*", ErrorMessage = "Некорректный ввод")]
        public int Months { get; set; }

        [Required(ErrorMessage = "Не указан счет")]
        public string BalanceName { get; set; }
    }
}
