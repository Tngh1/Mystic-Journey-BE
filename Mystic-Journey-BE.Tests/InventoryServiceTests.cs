using AutoMapper;
using BLL.DTOs;
using BLL.Services;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Moq;
using System.Data;

namespace Mystic_Journey_BE.Tests;

public sealed class InventoryServiceTests
{
    [Theory]
    [InlineData("Necklace", "Shield")]
    [InlineData("Shield", "Necklace")]
    [InlineData("OffHand", "Necklace")]
    public async Task EquipItem_UnequipsExistingItemInSharedOffHandSlot(
        string equippedSlot,
        string newItemSlot)
    {
        const int playerProfileId = 7;

        var equippedItem = CreateInventoryItem(1, 101, playerProfileId, equippedSlot, isEquipped: true);
        var newItem = CreateInventoryItem(2, 102, playerProfileId, newItemSlot, isEquipped: false);
        var inventory = new List<InventoryItem> { equippedItem, newItem };

        var inventoryRepository = new Mock<IInventoryRepository>();
        inventoryRepository.Setup(x => x.GetById(newItem.InventoryItemId)).ReturnsAsync(newItem);
        inventoryRepository.Setup(x => x.GetByPlayerId(playerProfileId)).ReturnsAsync(inventory);
        inventoryRepository.Setup(x => x.UpdateItem(It.IsAny<InventoryItem>()))
            .ReturnsAsync((InventoryItem item) => item);

        var statRepository = new Mock<IPlayerStatRepository>();
        statRepository.Setup(x => x.CreateSnapshot(It.IsAny<PlayerStatsSnapshot>()))
            .ReturnsAsync((PlayerStatsSnapshot snapshot) => snapshot);

        var transactionManager = new Mock<ITransactionManager>();
        transactionManager
            .Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<IsolationLevel>()))
            .Returns((Func<Task> action, IsolationLevel _) => action());
        transactionManager
            .Setup(x => x.ExecuteInTransactionAsync(
                It.IsAny<Func<Task<(InventoryItem, PlayerStatsSnapshot?)>>>()))
            .Returns((Func<Task<(InventoryItem, PlayerStatsSnapshot?)>> action) => action());

        var mapper = new Mock<IMapper>();
        mapper.Setup(x => x.Map<InventoryItemResponseDto>(It.IsAny<object>()))
            .Returns(new InventoryItemResponseDto());

        var service = new InventoryService(
            inventoryRepository.Object,
            mapper.Object,
            statRepository.Object,
            new Mock<IPlayerProfileRepository>().Object,
            transactionManager.Object,
            new Mock<ICharacterService>().Object);

        await service.EquipItem(
            playerProfileId,
            new EquipItemRequestDto { InventoryItemId = newItem.InventoryItemId });

        Assert.False(equippedItem.IsEquipped);
        Assert.Null(equippedItem.EquippedSlot);
        Assert.True(newItem.IsEquipped);
        Assert.Equal(newItemSlot, newItem.EquippedSlot);
        Assert.Single(inventory, item => item.IsEquipped);
        inventoryRepository.Verify(x => x.UpdateItem(equippedItem), Times.Once);
    }

    private static InventoryItem CreateInventoryItem(
        int inventoryItemId,
        int itemId,
        int playerProfileId,
        string slot,
        bool isEquipped)
    {
        return new InventoryItem
        {
            InventoryItemId = inventoryItemId,
            ItemId = itemId,
            PlayerProfileId = playerProfileId,
            Quantity = 1,
            IsEquipped = isEquipped,
            EquippedSlot = isEquipped ? slot : null,
            Item = new Item
            {
                ItemId = itemId,
                Slot = slot,
                EquipmentStats = new EquipmentStats()
            }
        };
    }
}
