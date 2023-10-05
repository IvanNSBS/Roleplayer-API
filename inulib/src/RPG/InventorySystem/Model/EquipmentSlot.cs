using System.Linq;

namespace INUlib.RPG.InventorySystem
{
    public class EquipmentSlot<T> where T : class, IEquippableItem
    {
        #region Fields
        private T _itemInSlot;
        private int _slotId;
        private bool _deactivated;
        #endregion
        
        #region Constructor
        public EquipmentSlot(int slotId)
        {
            _slotId = slotId;
        }
        #endregion
        
        #region Methods
        public bool AcceptsItemType(int[] itemTypeId) => itemTypeId != null && itemTypeId.Contains(_slotId);
        public bool AcceptsItemType(int itemTypeId) => itemTypeId == _slotId;

        /// <summary>
        /// Unequips the currently equipped item in this slot.
        /// </summary>
        /// <returns>
        /// The item that was unequipped or null if there was no item in this slot.
        /// </returns>
        public T UnequipItem()
        {
            if(_itemInSlot == null)
                return null;
            
            T unequipped = _itemInSlot;

            _itemInSlot = null;
            return unequipped;
        }

        /// <summary>
        /// Equips an item in this slot.
        /// </summary>
        /// <param name="newItem">The item to be equipped to this slot</param>
        /// <returns>
        /// The item that was previously equipped in this slot if there was something in it and the item was equipped.
        /// The item that is currently being equipped if it was equipped and there was no item previously equipped.
        /// Null if the item was not equipped due to incompatible slots or disabled equipment slot.
        /// </returns>
        public T EquipItem(T newItem)
        {
            if (_deactivated)
                return null;
            
            if (!AcceptsItemType(newItem.TargetSlotIds))
                return null;
            
            T oldItem = _itemInSlot;
            _itemInSlot = newItem;

            if (oldItem == null)
                return _itemInSlot;
            
            return oldItem;
        }

        /// <summary>
        /// Tries to deactivate the item slot.
        /// </summary>
        /// <returns>
        /// True if there was no item equipped and the slot could be deactivated.
        /// False otherwise</returns>
        public bool TryDeactivate()
        {
            if (!CanBeDeactivated())
                return false;

            return _deactivated = true;
        }

        /// <summary>
        /// Reactivates the item slot. Has no effect if the slot was already
        /// activated.
        /// </summary>
        public void Activate() => _deactivated = false;
        
        /// <summary>
        /// Checks if this slot is currenctly activated
        /// </summary>
        /// <returns>True if it is active, false otherwise</returns>
        public bool IsSlotActive() => !_deactivated;
        
        /// <summary>
        /// Gets the item that is currently equipped in this slot.
        /// </summary>
        /// <returns>The item that is currently equipped in this slot or null if no item is equipped</returns>
        public IEquippableItem GetEquippedItem() => _itemInSlot;
        /// <summary>
        /// Checks if this slot has some item equipped in it
        /// </summary>
        /// <returns>True if there is an item equipped. False otherwise</returns>
        public bool HasItemEquipped() => _itemInSlot != null;
        
        /// <summary>
        /// Checks if the item slot can be deactivated
        /// </summary>
        /// <returns>True if there is no item equipped in this slot. False otherwise</returns>
        public bool CanBeDeactivated() => !HasItemEquipped();
        #endregion
    }
}