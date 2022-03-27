using System.ComponentModel.DataAnnotations;

namespace Lab1.Models.Entities
{
    public class Balance
    {
        [Key] public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public double Money { get; set; }
        public string ClientId { get; set; }
    }
}
