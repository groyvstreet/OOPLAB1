using Lab1.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace Lab1.Models.BalanceModels
{
    public class TransferBalanceModel
    {
        [Required(ErrorMessage = "Не указана сумма")]
        [RegularExpression(@"[0](?:[.][0-9][1-9]|[.][1-9])?|[1-9]+[0-9]{0,9}(?:[.][0-9][1-9]|[.][1-9])?", ErrorMessage = "Некорректный ввод")]
        public string Money { get; set; }

        [Required(ErrorMessage = "Не указан email")]
        [EmailAddress(ErrorMessage = "Некорректный адрес")]
        public string EmailTo { get; set; }

        [Required(ErrorMessage = "Не указано название банка")]
        [RegularExpression(@"(?:[A-Za-zА-Яа-я0-9]+(?:\s[A-Za-zА-Яа-я0-9])?)+", ErrorMessage = "Некорректный ввод")]
        public string BankNameTo { get; set; }

        [Required(ErrorMessage = "Не указано название счета")]
        [RegularExpression(@"(?:[A-Za-zА-Яа-я0-9]+(?:\s[A-Za-zА-Яа-я0-9])?)+", ErrorMessage = "Некорректный ввод")]
        public string BalanceNameTo { get; set; }

        public string IdFrom { get; set; }
        public List<Balance> Balances { get; set; }
    }
}
