using INUlib.Core.Math;
using INUlib.RPG.ItemSystem;

namespace RPG.InventorySystem.View
{
    public interface IContainerView<TItem> where TItem : IItemInstance
    {
        void OnItemAdded(TItem item, int2 gridCoord);
        void OnItemRemoved(TItem item, int2 gridCoord);
    }
}