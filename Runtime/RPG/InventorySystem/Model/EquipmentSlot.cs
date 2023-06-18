﻿using System.Linq;

namespace INUlib.RPG.InventorySystem
{
    public class EquipmentSlot<T> where T : class, IEquippableItem
    {
        #region Fields
        private T _itemInSlot;
        private int _slotId;
        #endregion
        
        #region Constructor
        public EquipmentSlot(int slotId)
        {
            _slotId = slotId;
        }
        #endregion
        
        #region Methods
        public bool AcceptsItemType(int[] itemTypeId)
        {
            return itemTypeId != null && itemTypeId.Contains(_slotId);
        }

        public bool AcceptsItemType(int itemTypeId) => itemTypeId == _slotId;

        public T UnequipItem()
        {
            if(_itemInSlot == null)
                return null;
            
            T unequipped = _itemInSlot;

            _itemInSlot = null;
            return unequipped;
        }

        public T EquipItem(T newItem)
        {
            if (!AcceptsItemType(newItem.TargetSlotIds))
                return null;
            
            T oldItem = _itemInSlot;
            _itemInSlot = newItem;

            if (oldItem == null)
                return _itemInSlot;
            
            return oldItem;
        }

        public IEquippableItem GetEquippedItem() => _itemInSlot;
        public bool HasItemEquipped() => _itemInSlot != null;
        #endregion
    }
}