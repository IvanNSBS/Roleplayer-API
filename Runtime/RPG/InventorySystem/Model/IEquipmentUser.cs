namespace INUlib.RPG.InventorySystem
{
    public interface IEquipmentUser<T> where T : IEquippableItem
    {
        void OnItemEquipped(T item);
        void OnItemUnequipped(T item);
    }
}