using System;

namespace Lib.DataStructures.Heap.Interfaces
{
	public interface IHeapItem<T> : IComparable<T>
	{
		int HeapIndex { get; set; }
	}
}
