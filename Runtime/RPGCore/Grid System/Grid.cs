using UnityEngine;
using System.Collections.Generic;

namespace RPGCore.GridSystem
{
    public abstract class Grid<T>
    {
        #region Fields
        protected Vector2Int gridSize = new Vector2Int(1, 1);
        protected float celSize = 1;
        protected T[,] gridItems;
        #endregion Fields
        
        
        #region Grid Properties
        
        /// <summary>
        /// Method that will return the grid origin position
        /// </summary>
        /// <returns>Grid origin position. Local or World depends on implementation</returns>
        public abstract Vector3 GridOrigin { get; }
        
        #endregion Grid Properties
        
        
        #region Constructors
        protected Grid(Vector2Int gridSize, float celSize)
        {
            this.celSize = celSize;
            this.gridSize = gridSize;
        }
        
        #endregion Constructors
        
        
        #region Grid Methods
        /// <summary>
        /// Function called on grid 2D Array initialization.
        /// Creates the grid element on construction
        /// </summary>
        /// <param name="x">x position in the grid</param>
        /// <param name="y">y position in the grid</param>
        public abstract T InitGridCel(int x, int y);
        
        /// <summary>
        /// Helper Function that will convert a world position to local grid position
        /// </summary>
        /// <param name="worldPos">Position to convert</param>
        /// <returns>Position in local space</returns>
        public abstract Vector3 WorldPosToLocal(Vector3 worldPos);
        
        /// <summary>
        /// Helper function to get the grid element at a certain local grid position
        /// </summary>
        /// <param name="localPos">The local position to get the item</param>
        /// <returns>Must return a grid Element at localPos if localPos is a valid position. Null otherwise</returns>
        public abstract T LocalPosToGrid(Vector3 localPos);

        /// <summary>
        /// Helper function to convert a world position to grid cel
        /// </summary>
        /// <param name="worldPos">the world position</param>
        /// <returns>Must return a grid element at worldPos if worldPos is a valid position. Null otherwise</returns>
        public T WorldPosToGrid(Vector3 worldPos)
        {
            return LocalPosToGrid(WorldPosToLocal(worldPos));
        }
        #endregion Grid Methods
        
        
        #region Utility Methods

        protected void StartGrid()
        {
            gridItems = new T[gridSize.x, gridSize.y];
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    gridItems[x, y] = InitGridCel(x, y);
                }
            }
        }
        
        #endregion Utility Methods
    }
}