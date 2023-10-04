using System;

namespace INUlib.Gameplay.DataStructures.Heap.Interfaces
{
	public interface IHeapItem<T> : IComparable<T>
	{
		int HeapIndex { get; set; }
	}
}
