using UnityEngine;
using RPGCore.DataStructures;

namespace RPGCore.Grids.Algorithms
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