namespace Lab1.Models.Entities.Actions
{
    public class SalaryRejectingBySpecialistAction : Action
    {
        public string SpecialistId { get; set; }
        public string SpecialistEmail { get; set; }
        public string ClientId { get; set; }
        public string ClientEmail { get; set; }
        public string CompanyName { get; set; }
    }
}
