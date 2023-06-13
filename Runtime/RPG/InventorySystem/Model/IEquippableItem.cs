using INUlib.RPG.ItemSystem;

namespace INUlib.RPG.InventorySystem
{
    public interface IEquippableItem : IItemInstance
    {
        int SlotTypeId { get; }
    }
}