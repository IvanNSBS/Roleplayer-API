using INUlib.RPG.InventorySystem;
using INUlib.RPG.ItemSystem;
using Utils.Math;

namespace RPG.InventorySystem.View
{
    public interface IContainerView<TItem> where TItem : IItemInstance
    {
        void OnItemAdded(TItem item, int2 gridCoord);
        void OnItemRemoved(TItem item, int2 gridCoord);
    }
}