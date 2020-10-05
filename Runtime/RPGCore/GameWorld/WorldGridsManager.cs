using UnityEngine;

namespace RPGCore.GameWorld
{
    [ExecuteInEditMode]
    public class WorldGridsManager : MonoBehaviour
    {
        #region Singleton
        private static WorldGridsManager _instance;
        public static WorldGridsManager Instance { get => _instance; private set => _instance = value; }
        #endregion Singleton

        [SerializeField] Vector2Int _gridSize = new Vector2Int(128, 128);
        [SerializeField] float _celSize = 1;

        #region Fields
        private Pathfinder _pathfinder;
        private WorldGrid _grid;
        #endregion Fields


        #region MonoBehaviour Methods
        private void Awake()
        {
            if (!_instance)
            {
                Instance = this;
                // _pathfinder = new Pathfinder(this);
            }
            else
            {
                Destroy(this);
                return;
            }
        }

        private void OnValidate()
        {
            _grid = new WorldGrid(_gridSize, _celSize, transform);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireCube(transform.position, new Vector3(_grid.GridSize.x * _grid.CelSize, _grid.GridSize.y * _grid.CelSize, 0));
            if (_grid != null)
            {
                foreach (GridCel n in _grid.GridItems)
                {
                    Gizmos.color = ColorFromGridState(n);
                    Gizmos.DrawCube(n.WorldPos, new Vector3(_grid.CelSize - 0.05f, _grid.CelSize - 0.05f, 0f));
                }
            }
            
            CheckPositionFromMouse();
        }
        #endregion MonoBehaviour Methods


        #region Utility Methods
        private Color ColorFromGridState(GridCel cel)
        {
            switch (cel.celState)
            {
                case GridCelState.Free:
                    return new Color(0.6f, 1f, 0.6f, 1f);

                case GridCelState.Occupied:
                    return new Color(1.0f, 0.1f, 0.1f, 1);

                case GridCelState.Walkable:
                    return new Color(0.0f, 1.0f, 0.1f, 1f);

                case GridCelState.WalkablePlaced:
                    return new Color(0.4f, 0.4f, 0.05f, 1f);

                case GridCelState.NotWalkable:
                    return new Color(1.0f, 0.0f, 0.0f, 1f);

                default:
                    return new Color(1.0f, 1.0f, 1.0f, 1f);
            }
        }

        private void CheckPositionFromMouse()
        {
            Vector3 mouse = _grid.MouseToLocal();
            GridCel cel = _grid.LocalPosToGrid(mouse);
            if (cel != null)
            {
                Gizmos.color = Color.black; 
                Gizmos.DrawCube(cel.WorldPos, new Vector3(_grid.CelSize - 0.05f, _grid.CelSize - 0.05f, 0));
            }
        }
        
        #endregion Utility Methods
    }
}