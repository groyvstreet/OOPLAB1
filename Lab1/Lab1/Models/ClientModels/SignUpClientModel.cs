using System.ComponentModel.DataAnnotations;
using Lab1.Models.Entities;

namespace Lab1.Models.ClientModels
{
    public class SignUpClientModel
    {
        [Required(ErrorMessage = "Не указана серия паспорта")]
        [RegularExpression(@"[A-Z]{2}|[А-Я]{2}", ErrorMessage = "2 буквы")]
        public string? PassportSeries { get; set; }

        [Required(ErrorMessage = "Не указан номер паспорта")]
        [RegularExpression(@"[1-9]{7}", ErrorMessage = "7 цифр")]
        public int PassportNumber { get; set; }

        [Required(ErrorMessage = "Не указан идентификационный номер")]
        [RegularExpression(@"[0-9]{7}[A-Z][0-9]{3}[A-Z]{2}[0-9]|[0-9]{7}[А-Я][0-9]{3}[А-Я]{2}[0-9]", ErrorMessage = "ЦЦЦЦЦЦЦ Б ЦЦЦ ББ Ц (без пробелов)")]
        public string? IdentificationNumber { get; set; }
        public string CompanyId { get; set; }
        public List<Company> Companies { get; set; }
    }
}
