using UnityEngine;
using INUlib.Gameplay.Grids;
using INUlib.Gameplay.Grids.Algorithms.Interfaces;

namespace RPG.GameWorld
{
    [System.Serializable]
    public class WorldGrid : SquareGrid<IPath>
    {
        #region Constructors
        public WorldGrid(Vector2Int gridSize, float celSize, Transform transform) : base(gridSize, celSize, transform)
        {
            StartGrid();
        }
        #endregion Constructors
        
        #region Initialization Methods

        protected override IPath InitGridCel(int x, int y)
        {
            Vector3 celPos = GridOrigin + ToCenterOffset + new Vector3(CelSize * x, -CelSize * y);
            return new GridCel(GridCelState.Free, celPos, x, y);
        }

        #endregion Initialization Methods
    }
}