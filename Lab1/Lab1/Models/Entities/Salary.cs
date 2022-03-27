using System.ComponentModel.DataAnnotations;

namespace Lab1.Models.Entities
{
    public class Salary
    {
        [Key] public string Id { get; set; } = Guid.NewGuid().ToString();
        public double Money { get; set; }
        public bool ApprovedBySpecialist { get; set; }
        public bool ApprovedByOperator { get; set; }
        public string  ClientId { get; set; }
    }
}
