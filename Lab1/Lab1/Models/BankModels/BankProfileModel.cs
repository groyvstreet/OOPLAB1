namespace Lab1.Models.BankModels
{
    public class BankProfileModel
    {
        public string? Name { get; set; }
        public int Percent { get; set; }
        public string? BIC { get; set; }
        public bool Authenticated { get; set; }
    }
}
