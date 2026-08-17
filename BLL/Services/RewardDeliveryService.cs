using BLL.DTOs;
using BLL.Exceptions;
using BLL.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services
{
    // Executes core business logic for i reward delivery service.
    public sealed class RewardDeliveryService : IRewardDeliveryService
    {
        private const int MaxMailboxItemStack = 99;
        private readonly IInventoryService _inventoryService;
        private readonly IMailboxService _mailboxService;

        // Initialize this instance from inventory service and mailbox service and store inventory service and mailbox service for later operations.
        public RewardDeliveryService(
            IInventoryService inventoryService,
            IMailboxService mailboxService)
        {
            _inventoryService = inventoryService;
            _mailboxService = mailboxService;
        }

        // Process deliver item async using player profile id, item id, quantity, and reward source; it creates item to inventory, creates add, and sends mailbox by list id and guards invalid or unavailable states and translates operation failures.
        public async Task DeliverItemAsync(
            int playerProfileId,
            int itemId,
            int quantity,
            string rewardSource)
        {
            if (quantity <= 0)  // Reject zero or negative item quantities before any DB work
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
