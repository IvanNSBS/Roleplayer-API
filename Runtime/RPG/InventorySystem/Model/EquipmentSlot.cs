namespace INUlib.RPG.InventorySystem
{
    public abstract class EquipmentSlot<T1, T2> where T2 : IEquipmentUser where T1 : class, IEquippableItem<T2>
    {
        #region Fields
        private T1 _itemInSlot;
        private T2 _equipmentUser;
        #endregion
        
        #region Constructor
        public EquipmentSlot(T2 equipmentUser)
        {
            _equipmentUser = equipmentUser;
        }
        #endregion
        
        #region Abstract Methods
        public abstract void AcceptsItemType(int itemTypeId);
        #endregion
        
        #region Methods
        public IItem UnequipItem()
        {
            if(_itemInSlot == null)
                return null;
            
            T1 item = _itemInSlot;
            _itemInSlot.OnUnequip(_equipmentUser);

            _itemInSlot = null;
            return item;
        }

        public T1 EquipItem(T1 item)
        {
            T1 oldItem = _itemInSlot;
            oldItem?.OnUnequip(_equipmentUser);

            _itemInSlot = item;
            _itemInSlot.OnEquip(_equipmentUser);
            
            return oldItem;
        } 
        #endregion
    }
}