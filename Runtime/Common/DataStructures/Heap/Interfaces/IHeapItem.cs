using System;

namespace INUlib.Common.DataStructures.Heap.Interfaces
{
	public interface IHeapItem<T> : IComparable<T>
	{
		int HeapIndex { get; set; }
	}
}
