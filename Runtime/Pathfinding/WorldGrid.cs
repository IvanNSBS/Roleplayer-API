using UnityEngine;
using System.Collections.Generic;

namespace RoleplayerAPI.Pathfinding2D
{
    [ExecuteInEditMode]
    public class WorldGrid : MonoBehaviour
    {
        #region Singleton
        private static WorldGrid _instance;
        public static WorldGrid Instance { get => _instance; private set => _instance = value; }
        #endregion Singleton


        #region Fields
        [SerializeField] float _celSize = 1;
        [SerializeField] Vector2Int _gridSize = new Vector2Int(128, 128);

        private GridCel[,] _grid;
        private Pathfinder _pathfinder;
        #endregion Fields


        #region Properties
        /// <summary>
        /// Property to get the Grid Origin position, which is the top-left corner
        /// </summary>
        public Vector3 GridOrigin
        {
            get => transform.position + new Vector3(-_gridSize.x * _celSize / 2.0f, _gridSize.y * _celSize / 2.0f, 0);
        }

        public Vector2 GridBounds { get => new Vector2(_celSize * _gridSize.x, _celSize * _gridSize.y); }

        /// <summary>
        /// Function to offset the topleft position of a cell to the center
        /// </summary>
        public Vector3 ToCenterOffset { get => new Vector3(_celSize / 2.0f, -_celSize / 2.0f, 0); }

        public float CelSize { get => _celSize; }

        public int CelCount { get => _gridSize.x * _gridSize.y; }

        public Pathfinder Pathfinder { get => _pathfinder; }
        #endregion Properties


        #region MonoBehaviour Methods
        private void Awake()
        {
            if (!_instance)
            {
                Instance = this;
                _pathfinder = new Pathfinder(this);
            }
            else
            {
                Destroy(this);
                return;
            }
        }

        private void Start()
        {
            _grid = new GridCel[_gridSize.x, _gridSize.y];

            for (int y = 0; y < _gridSize.y; y++)
            {
                for (int x = 0; x < _gridSize.x; x++)
                {
                    Vector3 celPos = GridOrigin + ToCenterOffset + new Vector3(_celSize * x, -_celSize * y);
                    _grid[x, y] = new GridCel(VerifyPosition(celPos), celPos, x, y);
                }
            }
        }

        private void OnValidate()
        {
            _grid = new GridCel[_gridSize.x, _gridSize.y];

            for (int y = 0; y < _gridSize.y; y++)
            {
                for (int x = 0; x < _gridSize.x; x++)
                {
                    Vector3 celPos = GridOrigin + ToCenterOffset + new Vector3(_celSize * x, -_celSize * y);
                    _grid[x, y] = new GridCel(VerifyPosition(celPos), celPos, x, y);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireCube(transform.position, new Vector3(_gridSize.x * _celSize, _gridSize.y * _celSize, 0));
            if (_grid != null)
            {
                foreach (GridCel n in _grid)
                {
                    Gizmos.color = ColorFromGridState(n);
                    Gizmos.DrawCube(n.worldPos, new Vector3(_celSize - 0.05f, _celSize - 0.05f, 0f));
                }
            }
        }
        #endregion MonoBehaviour Methods


        #region MonoBehaviour Utility Methods
        private Color ColorFromGridState(GridCel cel)
        {
            switch (cel.celState)
            {
                case GridCelState.Free:
                    return new Color(1.0f, 1.0f, 1.0f, 0.4f);

                case GridCelState.Occupied:
                    return new Color(1.0f, 0.1f, 0.1f, 0.4f);

                case GridCelState.Walkable:
                    return new Color(0.0f, 1.0f, 0.1f, 0.4f);

                case GridCelState.WalkablePlaced:
                    return new Color(0.4f, 0.4f, 0.05f, 0.4f);

                case GridCelState.NotWalkable:
                    return new Color(1.0f, 0.0f, 0.0f, 0.4f);

                default:
                    return new Color(1.0f, 1.0f, 1.0f, 0.4f);
            }
        }
        #endregion MonoBehaviour Utility Methods


        #region Methods

        /// <summary>
        /// Function to verify if a grid position is valid(nothing is occupying it)
        /// durin grid construction
        /// </summary>
        /// <param name="celPos">The grid cel position</param>
        /// <returns>The corresponding cel state based on world setup</returns>
        private GridCelState VerifyPosition(Vector3 celPos)
        {
            var col = Physics2D.OverlapBox(celPos, new Vector2(CelSize * 0.9f, CelSize * 0.9f), 0, LayerMask.GetMask("Path"));
            if (col != null)
                return GridCelState.Walkable;

            col = Physics2D.OverlapBox(celPos, new Vector2(CelSize * 0.9f, CelSize * 0.9f), 0, LayerMask.GetMask("Placement"));
            if (col != null)
                return GridCelState.NotWalkable;

            col = Physics2D.OverlapBox(celPos, new Vector2(CelSize * 0.9f, CelSize * 0.9f), 0, LayerMask.GetMask("Structure"));
            if (col != null)
                return GridCelState.NotWalkable;

            return GridCelState.NotWalkable;
        }

        /// <summary>
        /// Function to convert a local grid position to a grid cel
        /// </summary>
        /// <param name="localPos">Local position coordinates</param>
        /// <returns>The corresponding grid or null if it wasn't a valid position</returns>
        public GridCel LocalPosToGrid(Vector3 localPos)
        {
            if (localPos.x > 0 && localPos.y > 0 && localPos.x <= GridBounds.x && localPos.y <= GridBounds.y)
            {
                Vector3 pos = localPos / _celSize;
                int x = (int)pos.x;// % _gridSize;
                int y = (int)pos.y;// % _gridSize;
                return _grid[x, y];
            }

            return null;
        }

        /// <summary>
        /// Function to convert a world position to a grid cel
        /// </summary>
        /// <param name="pos">The world position</param>
        /// <returns>The corresponding grid or null if it wasn't a valid position</returns>
        public GridCel WorldPosToGrid(Vector3 pos)
        {
            return LocalPosToGrid(WorldToLocal(pos));
        }

        /// <summary>
        /// Convert a world coordinates to  grid coordinates
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Vector3 WorldToLocal(Vector3 pos)
        {
            Vector3 localPos = pos - GridOrigin;
            localPos = new Vector3(localPos.x, -localPos.y, localPos.z);
            return localPos;
        }

        /// <summary>
        /// Function to convert the mouse position on screen to local grid position
        /// </summary>
        /// <returns>The mouse position in grid coordinates</returns>
        public Vector3 MouseToLocal()
        {
            return WorldToLocal(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }


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

                    int checkX = cel.gridX + x;
                    int checkY = cel.gridY + y;

                    if (checkX >= 0 && checkX < _gridSize.x && checkY >= 0 && checkY < _gridSize.y)
                        neighbours.Add(_grid[checkX, checkY]);

                }
            }

            return neighbours;
        }
        #endregion Methods
    }
}