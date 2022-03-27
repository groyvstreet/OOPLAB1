using System.ComponentModel.DataAnnotations;

namespace Lab1.Models.Entities
{
    public class User
    {
        [Key] public string? Id { get; set; } = Guid.NewGuid().ToString();
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Patronymic { get; set; }
        public string? PhoneNumber { get; set; }
        public string? RoleName { get; set; }
        public string? BankId { get; set; }
    }
}
