using BLL.DTOs;
using BLL.Exceptions;
using BLL.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services
{
    public sealed class RewardDeliveryService : IRewardDeliveryService
    {
        private const int MaxMailboxItemStack = 99;
        private readonly IInventoryService _inventoryService;
        private readonly IMailboxService _mailboxService;

        public RewardDeliveryService(
            IInventoryService inventoryService,
            IMailboxService mailboxService)
        {
            _inventoryService = inventoryService;
            _mailboxService = mailboxService;
        }

        public async Task DeliverItemAsync(
            int playerProfileId,
            int itemId,
            int quantity,
            string rewardSource)
        {
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity));

            try
            {
                await _inventoryService.AddItemToInventory(playerProfileId, itemId, quantity);
            }
            catch (InventoryCapacityExceededException)
            {
                var attachments = new List<SendMailboxRewardItemDto>();
                var remaining = quantity;
                while (remaining > 0)
                {
                    var stackQuantity = Math.Min(MaxMailboxItemStack, remaining);
                    attachments.Add(new SendMailboxRewardItemDto
                    {
                        ItemId = itemId,
                        Quantity = stackQuantity
                    });
                    remaining -= stackQuantity;
                }

                await _mailboxService.SendMailboxByListId(new SendMailboxByListIdDto
                {
                    PlayerProfileIds = new List<int> { playerProfileId },
                    Title = "Inventory Full - Reward Delivered",
                    Content = $"Your complete reward from {rewardSource} was sent here because your inventory was full.",
                    Type = "SystemReward",
                    AttachedItems = attachments,
                    ExpiredAt = null
                });
            }
        }
    }
}
