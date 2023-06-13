using INUlib.RPG.ItemSystem;
using System.Collections.Generic;

namespace INUlib.RPG.InventorySystem
{
    public class EquipmentsManager<TItem> where TItem : class, IEquippableItem
    {
        #region Fields
        private List<EquipmentSlot<TItem>> _equipmentSlots;
        private IEquipmentUser<TItem> _equipmentUser;
        #endregion
        
        #region Methods;
        public EquipmentsManager(List<EquipmentSlot<TItem>> slots, IEquipmentUser<TItem> user)
        {
            _equipmentUser = user;
            _equipmentSlots = slots;
        }

        private List<EquipmentSlot<TItem>> GetEquipmentSlots() => _equipmentSlots;
        private EquipmentSlot<TItem> GetSlot(int slotType) => _equipmentSlots.Find(x => x.AcceptsItemType(slotType));

        /// <summary>
        /// Checks if a certain item can be equipped at a certain item slot
        /// </summary>
        /// <param name="item">The item to be equipped</param>
        /// <param name="slotType">The slot to check if the item can be equipped to</param>
        /// <returns>
        /// CannotPlace if there's no slot for the item or the item is not IEquippable.
        /// WillReplace if the item is an IEquippable with the same slot type as the slot AND there is already
        /// another item at the slot.
        /// CanPlace if the item is an IEquippable with the same slot type as the slot AND there is no item in the slot
        /// </returns>
        public PlacementState CanItemBeEquippedAt(IItemInstance item, int slotType)
        {
            if (item is not TItem equippableItem)
                return PlacementState.CannotPlace;

            var slot = GetSlot(slotType);
            if(slot == null || !slot.AcceptsItemType(equippableItem.SlotTypeId))
                return PlacementState.CannotPlace;
            
            if (slot.HasItemEquipped())
                return PlacementState.WillReplace;
            
            return PlacementState.CanPlace;
        }

        /// <summary>
        /// Checks if the item is equippable and that there is a equipment slot for the item
        /// </summary>
        /// <param name="item">The item to be checked</param>
        /// <returns>
        /// True if the item is equippable and there is a slot that accepts it.
        /// False otherwise
        /// </returns>
        public bool HasSlotForItem(IItemInstance item)
        {
            if (item is not TItem equip)
                return false;

            return GetSlot(equip.SlotTypeId) != null;
        }

        /// <summary>
        /// Checks if there is a equipment slot for the given slot type in this equipments manager
        /// </summary>
        /// <param name="slotType">The slot to check</param>
        /// <returns>
        /// True if there is an equipment slot with this type in this equipments manager.
        /// False otherwise.
        /// </returns>
        public bool HasSlot(int slotType) => _equipmentSlots.Find(x => x.AcceptsItemType(slotType)) != null; 

        /// <summary>
        /// Tries to equip an item in the given slot.
        /// </summary>
        /// <param name="item">The item to equip in the given slot</param>
        /// <param name="slotType">The slot to try to equip the item to</param>
        /// <returns>
        /// Null if the item is not equippable or there is no slot for this item.
        /// Will return the input item if the item was equipped but didn't replace anything.
        /// If the input item replaced an already equipped one, the replaced item will
        /// be returned
        /// </returns>
        public TItem EquipItem(IItemInstance item, int slotType)
        {
            PlacementState canPlace = CanItemBeEquippedAt(item, slotType);
            if (canPlace == PlacementState.CannotPlace || !HasSlot(slotType))
                return null;

            TItem equippable = (TItem)item;
            var slot = GetSlot(slotType);
            var result = slot.EquipItem(equippable);
            
            if(result != null && result != equippable)
                _equipmentUser.OnItemUnequipped(result);
            _equipmentUser.OnItemEquipped(equippable);

            return result;
            
        }

        /// <summary>
        /// Tries to unequip the item automatically
        /// </summary>
        /// <param name="item">The item to be unequipped</param>
        /// <returns>
        /// True if the item was unequipped.
        /// False otherwise.
        /// </returns>
        public bool UnequipItem(IItemInstance item)
        {
            if (item is not TItem equippableItem)
                return false;

            int slotId = equippableItem.SlotTypeId;
            var slot = GetSlot(slotId);

            var equippedItem = slot.GetEquippedItem();
            if (equippedItem == null || equippedItem != equippableItem)
                return false;

            var unequipped = slot.UnequipItem();
            _equipmentUser.OnItemUnequipped(unequipped);
            return true;
        }

        /// <summary>
        /// Tries to unequip the item from the given slot.
        /// </summary>
        /// <param name="slotId">The equipment slot id</param>
        /// <returns>
        /// The item that was unequipped.
        /// Null if the slot didn't exist or there was no item at the given slot.
        /// </returns>
        public TItem UnequipItemFrom(int slotId)
        {
            if (!HasSlot(slotId))
                return null;
            
            var slot = GetSlot(slotId);
            if (!slot.HasItemEquipped())
                return null;

            var unequipped = slot.UnequipItem();
            _equipmentUser.OnItemUnequipped(unequipped);
            return unequipped;
        }
        
        /// <summary>
        /// Tries to auto equip an item, looking for a free slot to equip it
        /// </summary>
        /// <param name="item">The item to be auto equipped</param>
        /// <returns>
        /// True if the item was equippable and there was a slot without anything inside it for the equip.
        /// False otherwise
        /// </returns>
        public bool AutoEquipItem(IItemInstance item)
        {
            if (item is not TItem equip)
                return false;
                
            if (CanItemBeEquippedAt(equip, equip.SlotTypeId) != PlacementState.CanPlace)
                return false;

            var slot = GetSlot(equip.SlotTypeId);
            slot.EquipItem(equip);
            _equipmentUser.OnItemEquipped(equip);
            return true;
        }
        #endregion
    }
}