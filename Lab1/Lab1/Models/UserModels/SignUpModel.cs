using Lab1.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace Lab1.Models.UserModels
{
    public class SignUpModel
    {
        [Required(ErrorMessage = "Не указано имя")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Не указана фамилия")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Не указано отчество")]
        public string? Patronymic { get; set; }

        [Required(ErrorMessage = "Не указан номер телефона")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Не указана электронная почта")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Длина пароля должна быть от 6 до 50 символов")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        public string RoleName { get; set; }
        public string BankId { get; set; }
    }
}
