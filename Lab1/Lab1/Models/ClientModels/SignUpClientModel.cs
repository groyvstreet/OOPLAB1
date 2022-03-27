using System.ComponentModel.DataAnnotations;
using Lab1.Models.Entities;

namespace Lab1.Models.ClientModels
{
    public class SignUpClientModel
    {
        [Required(ErrorMessage = "Не указана серия паспорта")]
        //[StringLength(2, MinimumLength = 2, ErrorMessage = "2 буквы")]
        //[RegularExpression(@"[A-Z]|[А-Я]{2}", ErrorMessage = "2 буквы")]
        public string? PassportSeries { get; set; }

        //[Required(ErrorMessage = "Не указан номер паспорта")]
        //[StringLength(7, MinimumLength = 7, ErrorMessage = "7 цифр")]
        //[RegularExpression(@"[0-9]{7}", ErrorMessage = "7 цифр")]
        public int PassportNumber { get; set; }

        [Required(ErrorMessage = "Не указан идентификационный номер")]
        //[StringLength(14, MinimumLength = 14, ErrorMessage = "14 символов")]
        //[RegularExpression(@"[A-Z]|[А-Я]|[0-9]{14}", ErrorMessage = "14 символов: буквы и цифры")]
        public string? IdentificationNumber { get; set; }
        public string CompanyId { get; set; }
        public List<Company> Companies { get; set; }
    }
}
