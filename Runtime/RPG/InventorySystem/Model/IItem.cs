namespace INUlib.RPG.InventorySystem
{
    public interface IItem
    {
        int Id { get; }
        int SlotType { get; }
        
        int Width { get; }
        int Height { get; }

        string GetFullName();
        string GetDescription();
    }
}