using INUlib.RPG.ItemSystem;

namespace INUlib.RPG.InventorySystem
{
    public interface IMetaItem<TInstance> where TInstance : IItemInstance
    {
        int Id { get; }
        int SlotType { get; }
        
        int Width { get; }
        int Height { get; }

        string GetFullName();
        string GetDescription();

        TInstance CreateInstance();
    }
}