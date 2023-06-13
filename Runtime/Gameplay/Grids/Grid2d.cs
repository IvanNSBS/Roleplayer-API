using Utils.Math;
using System.Collections.Generic;

namespace INUlib.Gameplay.Grids
{
    /// <summary>
    /// Template for a square grid.
    /// A concrete implementation must be provided since
    /// the grid elements constructor may not be known
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Grid2d<T> where T : class
    {
        #region Fields
        private readonly int _width;
        private readonly int _height;
        private readonly float _celSize;
        private Dictionary<int2, T> _itemsInside;
        private float3 _gridCenter;
        #endregion Fields
        
        
        #region Properties
        public IReadOnlyDictionary<int2, T> GridItems => _itemsInside;

        /// <summary>
        /// Property to get the grid origin in local grid space. Origin is the top-left
        /// position
        /// </summary>
        /// <returns>
        /// Grid origin position
        /// </returns>
        public float3 TopLeft => new (_gridCenter.x - _width * 0.5f * _celSize, _gridCenter.y - _height * 0.5f * _celSize, 0);

        /// <summary>
        /// Getter for Grid width
        /// </summary>
        public int Width => _width;
        /// <summary>
        /// Getter for Grid height
        /// </summary>
        public int Height => _height;
        
        /// <summary>
        /// Public getter and setter for Grid celSize
        /// </summary>
        public float CelSize
        {
            get => _celSize;
        }

        /// <summary>
        /// Property to get the Grid XY Bounds
        /// </summary>
        public float2 GridBounds => new (_celSize * _width, _celSize * _height);
        
        /// <summary>
        /// Function to offset the topleft position of a cell to its center
        /// </summary>
        public float3 ToCenterOffset => new (_celSize * 0.5f, _celSize * 0.5f, 0);
        
        /// <summary>
        /// How many cels a grid has
        /// </summary>
        public int CelCount => _width * _height;
        #endregion Grid Properties
        
        
        #region Constructors
        public Grid2d(int width, int height, float celSize, float3 gridCenter)
        {
            _width= width;
            _height = height;
            _celSize = celSize;
            _gridCenter = gridCenter;

            _itemsInside = new Dictionary<int2, T>();
        }
        #endregion Constructors
        
        
        #region Grid Methods
        /// <summary>
        /// Helper Function that will convert a world position to local grid position
        /// </summary>
        /// <param name="worldPos">Position to convert</param>
        /// <returns>Position in local space</returns>
        public float3 WorldToLocal(float3 worldPos)
        {
            float3 localPos = worldPos - TopLeft;
            localPos = new float3(localPos.x, localPos.y, _gridCenter.z);
            return localPos;
        }

        /// <summary>
        /// Helper function to get the grid element at a certain local grid position
        /// </summary>
        /// <param name="localPos">The local position to get the item</param>
        /// <returns>Must return a grid Element at localPos if localPos is a valid position. Null otherwise</returns>
        public int2 LocalToCoord(float3 localPos)
        {
            float3 pos = localPos / _celSize;
            int x = (int)pos.x;// % (int)celSize;
            int y = (int)pos.y;// % (int)celSize;
            return new int2(x, y);
        }

        /// <summary>
        /// Helper function to convert a world position to grid cel
        /// </summary>
        /// <param name="worldPos">the world position</param>
        /// <returns>Must return a grid element at worldPos if worldPos is a valid position. Null otherwise</returns>
        public int2 WorldToGridCoord(float3 worldPos)
        {
            return LocalToCoord(WorldToLocal(worldPos));
        }

        public float3 GridCoordToWorld(int2 gridCoord)
        {
            float toCenter = _celSize * 0.5f;
            float3 offset = new float3(_celSize * gridCoord.x + toCenter, _celSize * gridCoord.y + toCenter, 0);
            return TopLeft + offset;
        }

        public float3 GridCoordToWorld(int x, int y) => GridCoordToWorld(new int2(x, y));
        
        public bool IsCellFree(int2 celCoord) => !_itemsInside.ContainsKey(celCoord);

        public bool IsCellFree(float3 worldPos) => IsCellFree(WorldToGridCoord(worldPos));

        public bool IsCellFree(int xCoord, int yCoord) => IsCellFree(new int2(xCoord, yCoord));

        public T GetItemAt(int2 celCoord)
        {
            if(_itemsInside.ContainsKey(celCoord))
                return _itemsInside[celCoord];
            
            return null;
        }

        public T GetItemAt(int x, int y) => GetItemAt(new int2(x, y));
        public T GetItemAt(float3 worldPos) => GetItemAt(WorldToGridCoord(worldPos));
        
        /// <summary>
        /// Sets the value of a cell item at the given coordinate.
        /// </summary>
        /// <param name="celCoord">The coordinate</param>
        /// <param name="item">The cell item value at the coordinate</param>
        /// <returns>True if the coordinate was valid and the value was updated. False otherwise</returns>
        public bool SetCellItem(int2 celCoord, T item)
        {
            if (celCoord.x < 0 || celCoord.y < 0)
                return false;
                
            if (_itemsInside.ContainsKey(celCoord))
                _itemsInside.Add(celCoord, item);
            else
                _itemsInside[celCoord] = item;
        
            return true;
        }

        /// <summary>
        /// Sets the value of a cell item at the given coordinate.
        /// </summary>
        /// <param name="x">Grid X coordinate</param>
        /// <param name="y">Grid Y coordinate</param>
        /// <param name="item">The cell item value at the coordinate</param>
        /// <returns>True if the coordinate was valid and the value was updated. False otherwise</returns>
        public bool SetCellItem(int x, int y, T item) => SetCellItem(new int2(x, y), item);

        /// <summary>
        /// Sets the value of a cell item at the given coordinate.
        /// </summary>
        /// <param name="worldPos">The world coordinates of the item that will be converted to a grid coordinate</param>
        /// <param name="item">The cell item value at the coordinate</param>
        /// <returns>True if the coordinate was valid and the value was updated. False otherwise</returns>
        public bool SetCellItem(float3 worldPos, T item) => SetCellItem(WorldToGridCoord(worldPos), item);
        #endregion Grid Methods
    }
}