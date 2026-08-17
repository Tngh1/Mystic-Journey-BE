using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Initializes a new default instance of the IRewardDeliveryService class.
    public interface IRewardDeliveryService
    {
        Task DeliverItemAsync(
            int playerProfileId,
            int itemId,
            int quantity,
            string rewardSource);
    }
}
