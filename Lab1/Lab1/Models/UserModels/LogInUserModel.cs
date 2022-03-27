using System.ComponentModel.DataAnnotations;

namespace Lab1.Models.UserModels
{
    public class LogInUserModel
    {
        [Required(ErrorMessage = "Не указан Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string BankId { get; set; }
    }
}
