using System.ComponentModel.DataAnnotations;

namespace Lab1.Models.Entities
{
    public class SalaryApproving
    {
        [Key] public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ClientId { get; set; }
    }
}
