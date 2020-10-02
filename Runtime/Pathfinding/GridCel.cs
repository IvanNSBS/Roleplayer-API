using System;
using UnityEngine;

namespace RoleplayerAPI.Pathfinding2D
{
    [Serializable]
    public enum GridCelState
    {
        Free = 1 << 0, // Nothing is in the Grid Cel
        Occupied = 1 << 1, // Something occupies the grid cel.
        Walkable = 1 << 2, // Not free, but pathfinding units can walk over it
        WalkablePlaced = 1 << 3, // Walkable, but there's something in the cel. Can't place anything here
        NotWalkable = 1 << 4 // Can place structures or things like that, but pathfinding wont consider
    }

    public class GridCel : IHeapItem<GridCel>
    {
        #region Fields
        public GridCelState celState = GridCelState.Free;
        public Vector3 worldPos; // Center of cel
        public int gridX, gridY;

        public int gCost;
        public int hCost;
        public int fCost { get => gCost + hCost; }
        public int HeapIndex { get; set; }

        public GridCel parent;
        #endregion Fields


        #region Constructors
        public GridCel(GridCelState state, Vector3 wPos, int x, int y)
        {
            this.celState = state;
            worldPos = wPos;
            gridX = x;
            gridY = y;

            gCost = hCost = 0;
        }
        #endregion Constructors


        #region Methods
        public int CompareTo(GridCel other)
        {
            int compare = fCost.CompareTo(other.fCost);
            if (compare == 0)
            {
                compare = hCost.CompareTo(other.hCost);
            }

            return -compare;
        }
        #endregion Methods
    }
}
