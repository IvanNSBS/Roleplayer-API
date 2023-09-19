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
        private EquipmentSlot<TItem> GetSlot(int[] itemSlotIds)
        {
            return _equipmentSlots.Find(x => x.AcceptsItemType(itemSlotIds));
        }
        private EquipmentSlot<TItem> GetSlot(int itemSlotIds)
        {
            return _equipmentSlots.Find(x => x.AcceptsItemType(itemSlotIds));
        }

        /// <summary>
        /// Checks if a certain item can be equipped at a certain item slot
        /// </summary>
        /// <param name="item">The item to be equipped</param>
        /// <param name="itemSlotId">The slot to check if the item can be equipped to</param>
        /// <returns>
        /// CannotPlace if there's no slot for the item or the item is not IEquippable.
        /// WillReplace if the item is an IEquippable with the same slot type as the slot AND there is already
        /// another item at the slot.
        /// CanPlace if the item is an IEquippable with the same slot type as the slot AND there is no item in the slot
        /// </returns>
        public PlacementState CanItemBeEquippedAt(IItemInstance item, int itemSlotId)
        {
            if (item is not TItem equippableItem)
                return PlacementState.CannotPlace;

            var slot = GetSlot( new [] { itemSlotId });
            if(slot == null || !slot.AcceptsItemType(equippableItem.TargetSlotIds))
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

            return GetSlot(equip.TargetSlotIds) != null;
        }

        /// <summary>
        /// Checks if there is a equipment slot for the given slot type in this equipments manager
        /// </summary>
        /// <param name="itemSlotsId">The slots to check</param>
        /// <returns>
        /// True if there is an equipment slot with this type in this equipments manager.
        /// False otherwise.
        /// </returns>
        public bool HasSlot(int itemSlotId)
        {
            return _equipmentSlots.Find(x => x.AcceptsItemType(new [] { itemSlotId })) != null;
        }
        
        /// <summary>
        /// Checks if there is a equipment slot for the given slot type in this equipments manager
        /// </summary>
        /// <param name="itemSlotsId">The slots to check</param>
        /// <returns>
        /// True if there is an equipment slot with this type in this equipments manager.
        /// False otherwise.
        /// </returns>
        public bool HasSlot(int[] itemSlotsId)
        {
            return _equipmentSlots.Find(x => x.AcceptsItemType(itemSlotsId)) != null;
        }

        /// <summary>
        /// Checks if the given slot is currently active
        /// </summary>
        /// <param name="itemSlotId">The item slot to check</param>
        /// <returns>
        /// True if the item slot is active. False if the item slot does not exist or is deactivated.
        /// </returns>
        public bool IsSlotActive(int itemSlotId)
        {
            if (!HasSlot(itemSlotId))
                return false;

            return GetSlot(itemSlotId).IsSlotActive();
        }

        /// <summary>
        /// Tries to equip an item in the given slot.
        /// </summary>
        /// <param name="item">The item to equip in the given slot</param>
        /// <param name="itemSlotId">The slot the item will be equipped to</param>
        /// <returns>
        /// Null if the item is not equippable or there is no slot for this item.
        /// Will return the input item if the item was equipped but didn't replace anything.
        /// If the input item replaced an already equipped one, the replaced item will
        /// be returned
        /// </returns>
        public TItem EquipItem(IItemInstance item, int itemSlotId)
        {
            PlacementState canPlace = CanItemBeEquippedAt(item, itemSlotId);
            if (canPlace == PlacementState.CannotPlace || !HasSlot(itemSlotId))
                return null;

            TItem equip = (TItem)item;
            var occupiedSlots = GetOccupiedSlots(equip.DeactivateSlots, itemSlotId);
            int totalOccupiedSlotsCount = canPlace == PlacementState.WillReplace ? 1 + occupiedSlots.Count : occupiedSlots.Count;
            
            bool tooMuchOccupiedSlots = totalOccupiedSlotsCount > 1;
            if (tooMuchOccupiedSlots)
                return null;
            
            var slot = GetSlot(itemSlotId);
            var returnValue = equip;
            
            // Which slot has an item inside
            var unequipItemSlot = occupiedSlots.Count == 1 ? occupiedSlots[0] : slot;
            var unequipped = unequipItemSlot.UnequipItem();
            if (unequipped != null)
            {
                _equipmentUser.OnItemUnequipped(unequipped);
                returnValue = unequipped;
            }

            slot.EquipItem(equip);
            _equipmentUser.OnItemEquipped(equip);
            DeactivateSlots(equip.DeactivateSlots, itemSlotId);
            
            return returnValue;
        }

        /// <summary>
        /// Tries to unequip the item automatically
        /// </summary>
        /// <param name="item">The item to be unequipped</param>
        /// <returns>
        /// True if the item was equipped and thus could be unequipped.
        /// False otherwise.
        /// </returns>
        public bool UnequipItem(IItemInstance item)
        {
            if (item is not TItem equip)
                return false;

            var slot = FindSlotItemIsEquippedTo(item);
            if (slot == null)
                return false;

            var unequipped = slot.UnequipItem();
            _equipmentUser.OnItemUnequipped(unequipped);
            ActivateSlots(unequipped.DeactivateSlots);
            
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
            
            var slot = GetSlot(new[] { slotId });
            if (!slot.HasItemEquipped())
                return null;

            var unequipped = slot.UnequipItem();
            _equipmentUser.OnItemUnequipped(unequipped);
            ActivateSlots(unequipped.DeactivateSlots);

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
                    
            foreach (var slotId in equip.TargetSlotIds)
            {
                if (CanItemBeEquippedAt(equip, slotId) != PlacementState.CanPlace)
                    continue;

                var slot = GetSlot(slotId);
                
                bool allSlotsDeactivatable = AreAllSlotsDeactivatable(equip.DeactivateSlots, ignore: slotId);
                if (!allSlotsDeactivatable)
                    continue;
                
                slot.EquipItem(equip);
                _equipmentUser.OnItemEquipped(equip);
                DeactivateSlots(equip.DeactivateSlots, slotId);
                
                return true;
            }
            
            return false;
        }
        #endregion
        
        
        #region Helper Methods
        /// <summary>
        /// Helper method to check if there is at most one deactivatable slot and return it.
        /// </summary>
        /// <param name="slotsToDeactivate">The deactivatable slots to check</param>
        /// <param name="ignore">A slot to ignore</param>
        /// <returns>
        /// The only deactivatable slot available or null if all slots are not deactivatable
        /// </returns>
        private List<EquipmentSlot<TItem>> GetOccupiedSlots(int[] slotsToDeactivate, int ignore)
        {
            List<EquipmentSlot<TItem>> occupied = new List<EquipmentSlot<TItem>>();
            
            if (slotsToDeactivate == null)
                return occupied;

            foreach (int slotId in slotsToDeactivate)
            {
                if (!HasSlot(slotId) || slotId == ignore)
                    continue;

                var slot = GetSlot(slotId);
                if (slot.HasItemEquipped())
                    occupied.Add(slot);
            }

            return occupied;
        }

        private bool AreAllSlotsDeactivatable(int[] slotsToDeactivate, int ignore)
        {
            if (slotsToDeactivate == null || slotsToDeactivate.Length == 0)
                return true;

            bool allAreDeactivatable = true;
            foreach (int slotId in slotsToDeactivate)
            {
                if (!HasSlot(slotId) || slotId == ignore)
                    continue;

                if (!GetSlot(slotId).CanBeDeactivated())
                    allAreDeactivatable = false;
            }

            return allAreDeactivatable;
        }

        private void DeactivateSlots(int[] slotsToDeactivate, int ignore)
        {
            if (slotsToDeactivate == null || slotsToDeactivate.Length == 0)
                return;

            foreach (int slotId in slotsToDeactivate)
            {
                if (slotId == ignore)
                    continue;

                GetSlot(slotId)?.TryDeactivate();
            }
        }
        
        private void ActivateSlots(int[] deactivatedSlots)
        {
            if (deactivatedSlots == null || deactivatedSlots.Length == 0)
                return;

            foreach (int slotId in deactivatedSlots)
                GetSlot(slotId)?.Activate();
        }

        private EquipmentSlot<TItem> FindSlotItemIsEquippedTo(IItemInstance item)
        {
            if (item is not TItem equip)
                return null;

            return _equipmentSlots.Find(x => x.GetEquippedItem() == equip);
        }
        #endregion
    }
}