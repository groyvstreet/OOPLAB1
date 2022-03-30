using System.ComponentModel.DataAnnotations;

namespace Lab1.Models.BalanceModels
{
    public class OpenBalanceModel
    {
        [Required(ErrorMessage = "Не указано название счета")]
        [RegularExpression(@"(?:[A-Za-zА-Яа-я0-9]+(?:\s[A-Za-zА-Яа-я0-9])?)+", ErrorMessage = "Некорректный ввод")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Не указана сумма")]
        [RegularExpression(@"[0](?:[.][0-9][1-9]|[.][1-9])?|[1-9]+[0-9]{0,9}(?:[.][0-9][1-9]|[.][1-9])?", ErrorMessage = "Некорректный ввод")]
        public double Money { get; set; }
    }
}
