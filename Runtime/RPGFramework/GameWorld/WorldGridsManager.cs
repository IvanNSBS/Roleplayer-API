using UnityEngine;
using System.Linq;
using RPGCore.Grids.Algorithms;
using UnityEditor;

namespace RPGFramework.GameWorld
{
    [ExecuteInEditMode]
    public class WorldGridsManager : MonoBehaviour
    {
        #region Fields
        private Pathfinding m_pathfinding;
        private WorldGrid m_squareGrid;
        [SerializeField] Vector2Int _gridSize = new Vector2Int(128, 128);
        [SerializeField] float _celSize = 1;
        #endregion Fields

        #region Singleton
        private static WorldGridsManager _instance;
        public static WorldGridsManager Instance { get => _instance; private set => _instance = value; }
        #endregion Singleton
        

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
            m_squareGrid = new WorldGrid(_gridSize, _celSize, transform);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireCube(transform.position, new Vector3(m_squareGrid.GridSize.x * m_squareGrid.CelSize, m_squareGrid.GridSize.y * m_squareGrid.CelSize, 0));
            if (m_squareGrid != null)
            {
                foreach (GridCel n in m_squareGrid.GridItems)
                {
                    Gizmos.color = ColorFromGridState(n);
                    Gizmos.DrawCube(n.WorldPos, new Vector3(m_squareGrid.CelSize - 0.05f, m_squareGrid.CelSize - 0.05f, 0f));
                }
            }
            
            CheckPositionFromMouse();
            FindPathTest();
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
            Vector3 mouse = MouseToLocal();
            IPath cel = m_squareGrid.LocalPosToGrid(mouse);
            if (cel != null)
            {
                Gizmos.color = Color.black; 
                Gizmos.DrawCube(cel.WorldPos, new Vector3(m_squareGrid.CelSize - 0.05f, m_squareGrid.CelSize - 0.05f, 0));
            }
        }

        /// <returns>The mouse position in grid coordinates</returns>
        public Vector3 MouseToLocal()
        {
            return m_squareGrid.WorldPosToLocal(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }


        [SerializeField] private Transform target;
        public void FindPathTest()
        {
            if (!Application.isPlaying)
                return;
            Pathfinding pathfinder = new RPGCore.Grids.Algorithms.Pathfinding(Instance.m_squareGrid);
            var path = pathfinder.FindPath(Camera.main.ScreenToWorldPoint(Input.mousePosition), target.position);
            if (path == null)
                return;
            var listPath = path.ToList();
            
            Gizmos.color = Color.yellow;
            foreach (GridCel n in listPath)
            {
                Gizmos.DrawCube(n.WorldPos, new Vector3(m_squareGrid.CelSize - 0.05f, m_squareGrid.CelSize - 0.05f, 0f));
            }
        }
        #endregion Utility Methods
    }
}