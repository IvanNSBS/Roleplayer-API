namespace INUlib.RPG.InventorySystem
{
    public class EquipmentSlot<T> where T : class, IEquippableItem
    {
        #region Fields
        private T _itemInSlot;
        private IEquipmentUser<T> _equipmentUser;
        private int _slotId;
        #endregion
        
        #region Constructor
        public EquipmentSlot(IEquipmentUser<T> equipmentUser, int slotId)
        {
            _equipmentUser = equipmentUser;
            _slotId = slotId;
        }
        #endregion
        
        #region Abstract Methods
        public bool AcceptsItemType(int itemTypeId) => itemTypeId == _slotId;
        #endregion
        
        
        #region Methods
        public T UnequipItem()
        {
            if(_itemInSlot == null)
                return null;
            
            T unequipped = _itemInSlot;
            _equipmentUser.OnItemUnequipped(unequipped);

            _itemInSlot = null;
            return unequipped;
        }

        public T EquipItem(T newItem)
        {
            if (!AcceptsItemType(newItem.SlotTypeId))
                return null;
            
            T oldItem = _itemInSlot;
            if(oldItem != null)
                _equipmentUser.OnItemUnequipped(oldItem);

            _itemInSlot = newItem;
            _equipmentUser.OnItemEquipped(newItem);

            if (oldItem == null)
                return _itemInSlot;
            
            return oldItem;
        }

        public IEquippableItem GetEquippedItem() => _itemInSlot;
        public bool HasItemEquipped() => _itemInSlot != null;
        #endregion
    }
}