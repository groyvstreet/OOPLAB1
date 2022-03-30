using Lab1.Models.Entities;

namespace Lab1.Models.ManagerModels
{
    public class BalanceTransferApprovingModel
    {
        public string ApprovingId { get; set; }
        public double Money { get; set; }
        public Specialist SpecialistFrom { get; set; }
        public Specialist SpecialistTo { get; set; }
    }
}
