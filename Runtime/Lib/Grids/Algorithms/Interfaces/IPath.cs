using UnityEngine;
using Lib.DataStructures.Heap.Interfaces;

namespace Lib.Grids.Algorithms.Interfaces
{
    public interface IPath : IHeapItem<IPath>
    {
        int GridX { get; }
        int GridY { get; }
        int FCost { get; }
        int GCost { get; set; }
        int HCost { get; set; }
        IPath Parent { get; set; }
        Vector3 WorldPos { get; }
    }
}