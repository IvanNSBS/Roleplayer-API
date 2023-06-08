namespace INUlib.RPG.InventorySystem
{
    public interface IEquippableItem<T> : IItem where T : IEquipmentUser
    {
        int SlotId { get; }
        void OnEquip(T user);
        void OnUnequip(T user);
    }
}