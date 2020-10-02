using System;

namespace RoleplayerAPI.Pathfinding2D
{
	public interface IHeapItem<T> : IComparable<T>
	{
		int HeapIndex { get; set; }
	}
}
