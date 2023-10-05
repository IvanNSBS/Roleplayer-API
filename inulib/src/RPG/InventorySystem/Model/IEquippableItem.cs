using INUlib.RPG.ItemSystem;

namespace INUlib.RPG.InventorySystem
{
    public interface IEquippableItem : IItemInstance
    {
        /// <summary>
        /// Which slots ID this item can be equipped to.
        /// </summary>
        int[] TargetSlotIds { get; }
        
        /// <summary>
        /// Those slots must be free to equip the item and they will be deactivated(you wont be able to equip items in them)
        /// while this item is equipped.
        /// If set to null or an empty array, this effect will be disabled.
        ///Use case: A two handed weapon that requires the other hand to be free and disables
        /// the other hand to be equipped.
        /// </summary>
        int[] DeactivateSlots { get; }
    }
}