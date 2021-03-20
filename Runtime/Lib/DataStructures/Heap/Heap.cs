using Lib.DataStructures.Heap.Interfaces;

namespace Lib.DataStructures.Heap
{
	public class Heap<T> where T : IHeapItem<T>
	{
		#region Fields
		private T[] m_items;
		private int m_currentItemCount;
		#endregion Field

		
		#region Constructors
		public Heap(int maxHeapSize)
		{
			m_items = new T[maxHeapSize];
		}
		#endregion Constructors


		#region Methodss
		public void Add(T item)
		{
			item.HeapIndex = m_currentItemCount;
			m_items[m_currentItemCount] = item;
			HeapifyUp(item);
			m_currentItemCount++;
		}

		public T RemoveFirst()
		{
			T firstItem = m_items[0];
			m_currentItemCount--;
			m_items[0] = m_items[m_currentItemCount];
			m_items[0].HeapIndex = 0;
			HeapifyDown(m_items[0]);
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
				return m_currentItemCount;
			}
		}

		public bool Contains(T item)
		{
			return Equals(m_items[item.HeapIndex], item);
		}

		void HeapifyDown(T item)
		{
			while (true)
			{
				int childIndexLeft = item.HeapIndex * 2 + 1;
				int childIndexRight = item.HeapIndex * 2 + 2;
				int swapIndex = 0;

				if (childIndexLeft < m_currentItemCount)
				{
					swapIndex = childIndexLeft;

					if (childIndexRight < m_currentItemCount)
						if (m_items[childIndexLeft].CompareTo(m_items[childIndexRight]) < 0)
							swapIndex = childIndexRight;

					if (item.CompareTo(m_items[swapIndex]) < 0)
						Swap(item, m_items[swapIndex]);
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
				T parentItem = m_items[parentIndex];
				if (item.CompareTo(parentItem) > 0)
					Swap(item, parentItem);
				else
					break;

				parentIndex = (item.HeapIndex - 1) / 2;
			}
		}

		void Swap(T itemA, T itemB)
		{
			m_items[itemA.HeapIndex] = itemB;
			m_items[itemB.HeapIndex] = itemA;
			int itemAIndex = itemA.HeapIndex;
			itemA.HeapIndex = itemB.HeapIndex;
			itemB.HeapIndex = itemAIndex;
		}
		#endregion Methods
	}
}