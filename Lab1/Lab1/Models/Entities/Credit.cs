using System.ComponentModel.DataAnnotations;

namespace Lab1.Models.Entities
{
    public class Credit
    {
        [Key] public string Id { get; set; } = Guid.NewGuid().ToString();
        public double Money { get; set; }
        public double MoneyWithPercent { get; set; }
        public int Percent { get; set; }
        public int Fines { get; set; }
        public int Months { get; set; }
        public int PayedMonths { get; set; }
        public DateTime CreatingTime { get; set; }
        public DateTime PaymentTime { get; set; }
        public bool Approved { get; set; }
        public string ClientId { get; set; }
    }
}
