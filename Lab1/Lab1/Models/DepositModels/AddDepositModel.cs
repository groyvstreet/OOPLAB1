﻿using System.ComponentModel.DataAnnotations;

namespace Lab1.Models.DepositModels
{
    public class AddDepositModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Не указана сумма")]
        [RegularExpression(@"[0](?:[.][0-9][1-9]|[.][1-9])?|[1-9]+[0-9]{0,9}(?:[.][0-9][1-9]|[.][1-9])?", ErrorMessage = "Некорректный ввод")]
        public string Money { get; set; }
    }
}
