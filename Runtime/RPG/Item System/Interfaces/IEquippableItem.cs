namespace RPG.ItemSystem
{
    public interface IEquippableItem : IItem
    {
        void OnEquip();
        void OnUnequip();
    }
}