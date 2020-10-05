﻿using UnityEngine;
using System.Collections.Generic;
using RPGCore.GridSystem;

namespace RPGCore.GameWorld
{
    public class WorldGrid : Grid<GridCel>
    {

        #region Fields
        private Transform userTransform;
        #endregion Fields

        #region Properties
        /// <summary>
        /// Public getter and setter for Grid gridSize
        /// </summary>
        public Vector2Int GridSize
        {
            get => gridSize;
            set => gridSize = value;
        }
        
        /// <summary>
        /// Public getter and setter for Grid celSize
        /// </summary>
        public float CelSize
        {
            get => celSize;
            set => celSize = value;
        }

        /// <summary>
        /// Public getter and setter for grid gridItems
        /// </summary>
        public GridCel[,] GridItems
        {
            get => gridItems;
            set => gridItems = value;
        }

        /// <summary>
        /// Property to get the Grid XY Bounds
        /// </summary>
        public Vector2 GridBounds { get => new Vector2(celSize * gridSize.x, celSize * gridSize.y); }
        
        /// <summary>
        /// Function to offset the topleft position of a cell to its center
        /// </summary>
        public Vector3 ToCenterOffset { get => new Vector3(celSize / 2.0f, -celSize / 2.0f, 0); }
        public int CelCount { get => gridSize.x * gridSize.y; }
        #endregion Properties
        
        #region Grid Properties
        /// <summary>
        /// World Grid origin is the top-left corner
        /// </summary>
        public override Vector3 GridOrigin
        {
            get => userTransform.position + new Vector3(-gridSize.x * celSize / 2.0f, gridSize.y * celSize / 2.0f, 0);
        }
        #endregion Grid Properties
        
        
        #region Constructors
        public WorldGrid(Vector2Int gridSize, float celSize, Transform transform) : base(gridSize, celSize)
        {
            userTransform = transform;
            StartGrid();
        }
        #endregion Constructors
        

        #region Methods
        public override GridCel InitGridCel(int x, int y)
        {
            Vector3 celPos = GridOrigin + ToCenterOffset + new Vector3(celSize * x, -celSize * y);
            return new GridCel(GridCelState.Free, celPos, x, y);
        }
        
        public override Vector3 WorldPosToLocal(Vector3 worldPos)
        {
            Vector3 localPos = worldPos - GridOrigin;
            localPos = new Vector3(localPos.x, -localPos.y, userTransform.position.z);
            return localPos;
        }
        
        public override GridCel LocalPosToGrid(Vector3 localPos)
        {
            if (localPos.x > 0 && localPos.y > 0 && localPos.x < GridBounds.x && localPos.y < GridBounds.y)
            {
                Vector3 pos = localPos / celSize;
                int x = (int)pos.x;// % (int)celSize;
                int y = (int)pos.y;// % (int)celSize;
                return gridItems[x, y];
            }

            return null;
        }
        #endregion Methods
        
        
        #region Utility Methods
        /// <summary>
        /// Function to get all the the neighbour Cels of a GridCel
        /// </summary>
        /// <param name="cel">GridCel which neighbours must be found</param>
        /// <returns>List with all the GridCels around the input Cel</returns>
        public List<GridCel> GetNeighbours(GridCel cel)
        {
            List<GridCel> neighbours = new List<GridCel>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0 || x == 1 && y == 1 || x == -1 && y == 1 || x == 1 && y == -1 || x == -1 && y == -1)
                        continue;

                    int checkX = cel.GridX + x;
                    int checkY = cel.GridY + y;

                    if (checkX >= 0 && checkX < gridSize.x && checkY >= 0 && checkY < gridSize.y)
                        neighbours.Add(gridItems[checkX, checkY]);
                }
            }

            return neighbours;
        }
        
        /// <summary>
        /// Function to convert the mouse position on screen to local grid position
        /// </summary>
        /// <returns>The mouse position in grid coordinates</returns>
        public Vector3 MouseToLocal()
        {
            return WorldPosToLocal(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
        #endregion Utility Methods
    }
}