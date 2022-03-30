using Lab1.Models.Entities;
using Lab1.Models.Entities.Actions;

namespace Lab1.Models.ManagerModels
{
    public class SpecialistBalanceTransferActionManagerModel
    {
        public List<BalanceTransferAction> BalanceTransferActions { get; set; }
        public string CurrentSpecialistId { get; set; }
    }
}
