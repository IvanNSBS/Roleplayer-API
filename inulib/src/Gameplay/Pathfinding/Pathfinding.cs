// using INUlib.Gameplay.Grids;
// using INUlib.Gameplay.Grids.Interfaces;
// using INUlib.Gameplay.DataStructures.Heap;
// using System.Collections.Generic;
// using INUlib.Gameplay.Grids.Algorithms.Interfaces;
// using INUlib.Core.Math;
// using System;

// namespace INUlib.Gameplay.Grids.Algorithms
// {
//     enum DistanceFunction
//     {
//         Manhattan,
//         Euclidean
//     }

//     public class Pathfinding
//     {
//         #region Fields
//         private SquareGrid<IPath> m_SquareGrid;
//         #endregion Fields

//         #region Constructor
//         public Pathfinding(SquareGrid<IPath> squareGrid)
//         {
//             m_SquareGrid = squareGrid;
//         }
//         #endregion Constructor

        
//         #region Methods
//         /// <summary>
//         /// Function to find a path between two positions
//         /// </summary>
//         /// <param name="startPos">starting position</param>
//         /// <param name="targetPos">target position</param>
//         /// <returns>Queue containing the nodes an entity shall traverse</returns>
//         public Queue<IPath> FindPath(float3 startPos, float3 targetPos)
//         {
//             IPath startCel = m_SquareGrid.WorldPosToGrid(startPos);
//             IPath targetCel = m_SquareGrid.WorldPosToGrid(targetPos);

//             if (targetCel == null || startCel == null)
//             {
//                 return null;
//             }

//             Heap<IPath> openSet = new Heap<IPath>(m_SquareGrid.CelCount);
//             HashSet<IPath> closedSet = new HashSet<IPath>();
//             openSet.Add(startCel);

//             while (openSet.Count > 0)
//             {
//                 IPath currentCel = openSet.RemoveFirst();
//                 closedSet.Add(currentCel);

//                 if (currentCel == targetCel)
//                 {
//                     var path = RetracePath(startCel, targetCel);
//                     return path;
//                 }

//                 foreach (IPath neighbour in GetNeighbours(currentCel))
//                 {
//                     // TODO: Consider neighour state 
//                     if (closedSet.Contains(neighbour))
//                         continue;

//                     int newCostToNeighbour = currentCel.GCost + GetDistance(currentCel, neighbour);
//                     if (newCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
//                     {
//                         neighbour.GCost = newCostToNeighbour;
//                         neighbour.HCost = GetDistance(neighbour, targetCel);
//                         neighbour.Parent = currentCel;

//                         if (!openSet.Contains(neighbour))
//                             openSet.Add(neighbour);
//                         else
//                             openSet.UpdateItem(neighbour);
//                     }
//                 }
//             }

//             return null;
//         }

//         /// <summary>
//         /// Function to find a path between a starting position and a grid entity
//         /// </summary>
//         /// <param name="startPos">starting position</param>
//         /// <param name="entity">target entity</param>
//         /// <returns>Queue containing the nodes an entity shall traverse</returns>
//         public Queue<IPath> FindPath(float3 startPos, IGridEntity entity)
//         {
//             var cels = entity.OccupyingCels;
//             List<IPath> neighbours = new List<IPath>();

//             foreach (IPath cel in cels)
//                 neighbours.AddRange(GetNeighbours(cel));

//             float distance = Mathf.Infinity;
//             float3 targetPos = Vector3.zero;

//             foreach (var neighbour in neighbours)
//             {
//                 // TODO: Consider neighbour state
//                 // if (neighbour.celState != IPathState.Walkable)
//                 //     continue;

//                 if (Vector3.Distance(neighbour.WorldPos, startPos) < distance)
//                 {
//                     distance = Vector3.Distance(neighbour.WorldPos, startPos);
//                     targetPos = neighbour.WorldPos;
//                 }
//             }
//             return FindPath(startPos, targetPos);
//         }

//         public Queue<IPath> RetracePath(IPath startCel, IPath endCel)
//         {
//             List<IPath> path = new List<IPath>();
//             IPath currentCel = endCel;
//             while (currentCel != startCel)
//             {
//                 path.Add(currentCel);
//                 currentCel = currentCel.Parent;
//             }

//             path.Reverse();
//             Queue<IPath> pathQueue = new Queue<IPath>(path);
//             return pathQueue;
//         }

//         private int GetDistance(IPath celA, IPath celB)
//         {
//             int dstX = Math.Abs(celA.GridX - celB.GridX);
//             int dstY = Math.Abs(celA.GridY - celB.GridY);
//             return dstX + dstY;
//         }
        
//         /// <summary>
//         /// Function to get all the the neighbour Cels of a IPath
//         /// </summary>
//         /// <param name="cel">IPath which neighbours must be found</param>
//         /// <returns>List with all the IPaths around the input Cel</returns>
//         public List<IPath> GetNeighbours(IPath cel)
//         {
//             List<IPath> neighbours = new List<IPath>();

//             for (int x = -1; x <= 1; x++)
//             {
//                 for (int y = -1; y <= 1; y++)
//                 {
//                     if (x == 0 && y == 0 || x == 1 && y == 1 || x == -1 && y == 1 || x == 1 && y == -1 || x == -1 && y == -1)
//                         continue;

//                     int checkX = cel.GridX + x;
//                     int checkY = cel.GridY + y;

//                     if (checkX >= 0 && checkX < m_SquareGrid.GridSize.x && checkY >= 0 && checkY < m_SquareGrid.GridSize.y)
//                         neighbours.Add(m_SquareGrid.GridItems[checkX, checkY]);
//                 }
//             }

//             return neighbours;
//         }
//         #endregion Methods
//     }
// }