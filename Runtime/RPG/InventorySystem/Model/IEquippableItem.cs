using INUlib.RPG.ItemSystem;

namespace INUlib.RPG.InventorySystem
{
    public interface IEquippableItem : IItemInstance
    {
        int[] TargetSlotIds { get; }
        
        /// <summary>
        /// When the item is equipped, all those other slots will need to be free
        /// and they will also be be filled on equipping the item.
        /// Use case: A two handed weapon that requires the other hand to be free
        /// to be equipped.
        /// To disable this functionality, this array can return either null or empty. 
        /// </summary>
        // int[] AlsoFilledSlots { get; }
    }
}