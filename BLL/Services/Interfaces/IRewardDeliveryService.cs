using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IRewardDeliveryService
    {
        Task DeliverItemAsync(
            int playerProfileId,
            int itemId,
            int quantity,
            string rewardSource);
    }
}
