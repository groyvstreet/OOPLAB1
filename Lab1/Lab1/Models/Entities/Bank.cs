using System.ComponentModel.DataAnnotations;

namespace Lab1.Models.Entities
{
    public class Bank
    {
        [Key] public string? Id { get; set; }
        public string? Name { get; set; }
        public int Percent { get; set; }
        public string? BIC { get; set; }
    }
}
