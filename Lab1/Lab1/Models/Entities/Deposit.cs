using System.ComponentModel.DataAnnotations;

namespace Lab1.Models.Entities
{
    public class Deposit
    {
        [Key] public string? Id { get; set; } = Guid.NewGuid().ToString();
        public double Money { get; set; }
        public int Percent { get; set; }
        public DateTime OpenedTime { get; set; } = DateTime.Now;
        public DateTime ClosedTime { get; set; }
        public bool Blocked { get; set; }
        public bool Freezed { get; set; }
        public string? ClientId { get; set; }
    }
}
