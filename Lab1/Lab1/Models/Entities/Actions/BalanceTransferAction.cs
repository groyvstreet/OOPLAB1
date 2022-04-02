using System.ComponentModel.DataAnnotations;

namespace Lab1.Models.Entities.Actions
{
    public class BalanceTransferAction : Action
    {
        public double Money { get; set; }
        public string BankIdFrom { get; set; }
        public string BankIdTo { get; set; }
        public string BankNameFrom { get; set; }
        public string BankNameTo { get; set; }
        public string UserIdTo { get; set; }
        public string UserEmailTo { get; set; }
        public string? BalanceIdFrom { get; set; }
        public string? BalanceIdTo { get; set; }
        public string BalanceNameFrom { get; set; }
        public string BalanceNameTo { get; set; }
    }
}
