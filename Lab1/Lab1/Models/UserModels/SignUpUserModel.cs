﻿using Lab1.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace Lab1.Models.UserModels
{
    public class SignUpUserModel
    {
        [Required(ErrorMessage = "Не указано имя")]
        [RegularExpression(@"[A-Z]{1}[a-z]*|[А-Я]{1}[а-я]*", ErrorMessage = "Имя должно содержать только буквы и начинаться с заглавной буквы")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Не указана фамилия")]
        [RegularExpression(@"[A-Z]{1}[a-z]*|[А-Я]{1}[а-я]*", ErrorMessage = "Фамилия должна содержать только буквы и начинаться с заглавной буквы")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Не указано отчество")]
        [RegularExpression(@"[A-Z]{1}[a-z]*|[А-Я]{1}[а-я]*", ErrorMessage = "Отчество должно содержать только буквы и начинаться с заглавной буквы")]
        public string? Patronymic { get; set; }

        [Required(ErrorMessage = "Не указан номер телефона")]
        [RegularExpression(@"^[+][3][7][5][0-9]{9}", ErrorMessage = "Некорректный номер")]
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
