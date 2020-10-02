using System.Collections.Generic;
using UnityEngine;

namespace RoleplayerAPI.Pathfinding2D
{
    enum DistanceFunction
    {
        Manhttan,
        Euclidean
    }

    public class Pathfinder
    {
        #region Fields
        private WorldGrid _grid;
        #endregion Fields

        #region Constructor
        public Pathfinder(WorldGrid grid)
        {
            _grid = grid;
        }
        #endregion Constructor

        #region Methods
        /// <summary>
        /// Function to find a path between two positions
        /// </summary>
        /// <param name="startPos">starting position</param>
        /// <param name="targetPos">target position</param>
        /// <returns>Queue containing the nodes an entity shall traverse</returns>
        public Queue<GridCel> FindPath(Vector3 startPos, Vector3 targetPos)
        {
            GridCel startCel = _grid.WorldPosToGrid(startPos);
            GridCel targetCel = _grid.WorldPosToGrid(targetPos);

            if (targetCel == null || startCel == null)
            {
                return null;
            }

            Heap<GridCel> openSet = new Heap<GridCel>(_grid.CelCount);
            HashSet<GridCel> closedSet = new HashSet<GridCel>();
            openSet.Add(startCel);

            while (openSet.Count > 0)
            {
                GridCel currentCel = openSet.RemoveFirst();
                closedSet.Add(currentCel);

                if (currentCel == targetCel)
                {
                    var path = RetracePath(startCel, targetCel);
                    return path;
                }

                foreach (GridCel neighbour in _grid.GetNeighbours(currentCel))
                {
                    if (neighbour.celState == GridCelState.NotWalkable || neighbour.celState == GridCelState.Occupied || closedSet.Contains(neighbour))
                        continue;

                    int newCostToNeighbour = currentCel.gCost + GetDistance(currentCel, neighbour);
                    if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetCel);
                        neighbour.parent = currentCel;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Function to find a path between a starting position and a grid entity
        /// </summary>
        /// <param name="startPos">starting position</param>
        /// <param name="entity">target entity</param>
        /// <returns>Queue containing the nodes an entity shall traverse</returns>
        public Queue<GridCel> FindPath(Vector3 startPos, IGridEntity entity)
        {
            var cels = entity.OccupyingCels;
            List<GridCel> neighbours = new List<GridCel>();

            foreach (GridCel cel in cels)
                neighbours.AddRange(_grid.GetNeighbours(cel));

            float distance = Mathf.Infinity;
            Vector3 targetPos = Vector3.zero;

            foreach (var neighbour in neighbours)
            {
                if (neighbour.celState != GridCelState.Walkable)
                    continue;

                if (Vector3.Distance(neighbour.worldPos, startPos) < distance)
                {
                    distance = Vector3.Distance(neighbour.worldPos, startPos);
                    targetPos = neighbour.worldPos;
                }
            }
            return FindPath(startPos, targetPos);
        }

        public Queue<GridCel> RetracePath(GridCel startCel, GridCel endCel)
        {
            List<GridCel> path = new List<GridCel>();
            GridCel currentCel = endCel;
            while (currentCel != startCel)
            {
                path.Add(currentCel);
                currentCel = currentCel.parent;
            }

            path.Reverse();
            Queue<GridCel> pathQueue = new Queue<GridCel>(path);
            return pathQueue;
        }

        int GetDistance(GridCel celA, GridCel celB)
        {
            int dstX = Mathf.Abs(celA.gridX - celB.gridX);
            int dstY = Mathf.Abs(celA.gridY - celB.gridY);
            return dstX + dstY;
        }
        #endregion Methods
    }
}