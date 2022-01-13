using INUlib.Gameplay.Grids;
using RPG.ItemSystem;
using UnityEngine;

namespace RPG.ContainerSystem
{
    public class ContainerGrid<TItem> : SquareGrid<ContainerSlot<TItem>> where TItem : IItem
    {
        #region Constructors
        
        public ContainerGrid(Vector2Int gridSize, float celSize, Transform transform) : base(gridSize, celSize, transform)
        {
            StartGrid();
        }
        
        #endregion Constructors
        
        
        #region Grid Methods

        protected override ContainerSlot<TItem> InitGridCel(int x, int y)
        {
            return new ContainerSlot<TItem>(x, y, m_transform);
        }
        
        #endregion Grid Methods
    }
}