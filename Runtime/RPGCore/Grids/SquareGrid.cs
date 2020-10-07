using UnityEngine;
using System;

namespace RPGCore.Grids
{
    /// <summary>
    /// Template for a square grid.
    /// A concrete implementation must be provided since
    /// the grid elements constructor may not be known
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SquareGrid<T>
    {
        #region Fields
        private readonly Vector2Int m_gridSize;
        private readonly float m_celSize;
        private T[,] m_gridItems;
        protected readonly Transform m_transform;
        #endregion Fields
        
        
        #region Properties
        
        /// <summary>
        /// Property to get the grid origin in local grid space. Origin is the top-left
        /// position
        /// </summary>
        /// <returns>Grid origin
        /// position</returns>
        public Vector3 GridOrigin
        {
            get => m_transform.position + new Vector3(-m_gridSize.x * m_celSize / 2.0f, m_gridSize.y * m_celSize / 2.0f, 0);
        }
        
        /// <summary>
        /// Public getter and setter for Grid gridSize
        /// </summary>
        public Vector2Int GridSize
        {
            get => m_gridSize;
        }
        
        /// <summary>
        /// Public getter and setter for Grid celSize
        /// </summary>
        public float CelSize
        {
            get => m_celSize;
        }

        /// <summary>
        /// Public getter and setter for grid gridItems
        /// </summary>
        public T[,] GridItems
        {
            get => m_gridItems;
            set => m_gridItems = value;
        }

        /// <summary>
        /// Property to get the Grid XY Bounds
        /// </summary>
        public Vector2 GridBounds { get => new Vector2(m_celSize * m_gridSize.x, m_celSize * m_gridSize.y); }
        
        /// <summary>
        /// Function to offset the topleft position of a cell to its center
        /// </summary>
        public Vector3 ToCenterOffset { get => new Vector3(m_celSize / 2.0f, -m_celSize / 2.0f, 0); }
        public int CelCount { get => m_gridSize.x * m_gridSize.y; }
        
        #endregion Grid Properties
        
        
        #region Constructors
        protected SquareGrid(Vector2Int gridSize, float celSize, Transform transform)
        {
            this.m_celSize = celSize;
            this.m_gridSize = gridSize;
            this.m_transform = transform;
        }
        #endregion Constructors
        
        
        #region Grid Methods

        /// <summary>
        /// Helper Function that will convert a world position to local grid position
        /// </summary>
        /// <param name="worldPos">Position to convert</param>
        /// <returns>Position in local space</returns>
        public Vector3 WorldPosToLocal(Vector3 worldPos)
        {
            Vector3 localPos = worldPos - GridOrigin;
            localPos = new Vector3(localPos.x, -localPos.y, m_transform.position.z);
            return localPos;
        }

        /// <summary>
        /// Helper function to get the grid element at a certain local grid position
        /// </summary>
        /// <param name="localPos">The local position to get the item</param>
        /// <returns>Must return a grid Element at localPos if localPos is a valid position. Null otherwise</returns>
        public T LocalPosToGrid(Vector3 localPos)
        {
            if (localPos.x > 0 && localPos.y > 0 && localPos.x < GridBounds.x && localPos.y < GridBounds.y)
            {
                Vector3 pos = localPos / m_celSize;
                int x = (int)pos.x;// % (int)celSize;
                int y = (int)pos.y;// % (int)celSize;
                return m_gridItems[x, y];
            }

            return default;
        }

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
        
        
        #region Initialization Methods

        protected void StartGrid()
        {
            m_gridItems = new T[m_gridSize.x, m_gridSize.y];
            for (int y = 0; y < m_gridSize.y; y++)
            {
                for (int x = 0; x < m_gridSize.x; x++)
                {
                    m_gridItems[x, y] = InitGridCel(x, y);
                }
            }
        }
        
        /// <summary>
        /// Function called on grid 2D Array initialization.
        /// Creates the grid element on construction
        /// </summary>
        /// <param name="x">x position in the grid</param>
        /// <param name="y">y position in the grid</param>
        protected virtual T InitGridCel(int x, int y)
        {
            return default;
        }
        #endregion Utility Methods
    }
}