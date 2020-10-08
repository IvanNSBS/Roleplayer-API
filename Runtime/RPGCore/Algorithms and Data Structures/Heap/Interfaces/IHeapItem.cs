using System;

namespace RPGCore.DataStructures
{
	public interface IHeapItem<T> : IComparable<T>
	{
		int HeapIndex { get; set; }
	}
}
