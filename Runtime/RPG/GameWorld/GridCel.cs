using System;
using INUlib.Gameplay.Grids.Algorithms.Interfaces;
using UnityEngine;

namespace RPG.GameWorld
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

    public class GridCel : IPath
    {
        #region Properties
        public GridCelState celState = GridCelState.Free;
        #endregion Properties

        #region IPath Properties
        public int HeapIndex { get; set; }
        public Vector3 WorldPos { get; private set; } // Cel Center
        public int GridX { get; private set; }
        public int GridY { get; private set; }
        public int GCost { get; set; }
        public int HCost { get; set; }
        public int FCost => GCost + HCost; 
        public IPath Parent { get; set; }

        #endregion IPath Properties

        
        #region Constructors
        public GridCel(GridCelState state, Vector3 wPos, int x, int y)
        {
            this.celState = state;
            WorldPos = wPos;
            GridX = x;
            GridY = y;

            GCost = HCost = 0;
        }
        #endregion Constructors


        #region Methods
        public int CompareTo(IPath other)
        {
            int compare = FCost.CompareTo(other.FCost);
            if (compare == 0)
            {
                compare = HCost.CompareTo(other.HCost);
            }

            return -compare;
        }
        #endregion Methods
    }
}
