using System;

namespace RoleplayerAPI.Pathfinding2D
{
	public class Heap<T> where T : IHeapItem<T>
	{
		#region Fields
		T[] items;
		int currentItemCount;
		#endregion Field


		#region Constructors
		public Heap(int maxHeapSize)
		{
			items = new T[maxHeapSize];
		}
		#endregion Constructors


		#region Methods
		public void Add(T item)
		{
			item.HeapIndex = currentItemCount;
			items[currentItemCount] = item;
			HeapifyUp(item);
			currentItemCount++;
		}

		public T RemoveFirst()
		{
			T firstItem = items[0];
			currentItemCount--;
			items[0] = items[currentItemCount];
			items[0].HeapIndex = 0;
			HeapifyDown(items[0]);
			return firstItem;
		}

		public void UpdateItem(T item)
		{
			HeapifyUp(item);
		}

		public int Count
		{
			get
			{
				return currentItemCount;
			}
		}

		public bool Contains(T item)
		{
			return Equals(items[item.HeapIndex], item);
		}

		void HeapifyDown(T item)
		{
			while (true)
			{
				int childIndexLeft = item.HeapIndex * 2 + 1;
				int childIndexRight = item.HeapIndex * 2 + 2;
				int swapIndex = 0;

				if (childIndexLeft < currentItemCount)
				{
					swapIndex = childIndexLeft;

					if (childIndexRight < currentItemCount)
						if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
							swapIndex = childIndexRight;

					if (item.CompareTo(items[swapIndex]) < 0)
						Swap(item, items[swapIndex]);
					else
						return;
				}
				else
					return;
			}
		}

		void HeapifyUp(T item)
		{
			int parentIndex = (item.HeapIndex - 1) / 2;

			while (true)
			{
				T parentItem = items[parentIndex];
				if (item.CompareTo(parentItem) > 0)
					Swap(item, parentItem);
				else
					break;

				parentIndex = (item.HeapIndex - 1) / 2;
			}
		}

		void Swap(T itemA, T itemB)
		{
			items[itemA.HeapIndex] = itemB;
			items[itemB.HeapIndex] = itemA;
			int itemAIndex = itemA.HeapIndex;
			itemA.HeapIndex = itemB.HeapIndex;
			itemB.HeapIndex = itemAIndex;
		}
	}
	#endregion Methods
}