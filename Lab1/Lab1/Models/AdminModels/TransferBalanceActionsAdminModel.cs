using Lab1.Models.Entities;
using Lab1.Models.Entities.Actions;

namespace Lab1.Models.AdminModels
{
    public class TransferBalanceActionsAdminModel
    {
        public List<BalanceTransferAction> BalanceTransferActions { get; set; }
        public string CurrentClientId { get; set; }
    }
}
