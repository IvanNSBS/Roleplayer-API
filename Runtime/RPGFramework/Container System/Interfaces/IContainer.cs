using UnityEngine;
using RPGCore.Grids;
using RPGFramework.ItemSystem;
using System.Collections.Generic;


namespace RPGFramework.ContainerSystem
{
    public interface IContainer<TItem> where TItem : IItem
    {
        List<TItem> ItemsInside { get; set; }
        Vector2Int ContainerSize { get; set; }
        ContainerGrid<TItem> ContainerGrid { get; set; }
        void AddItem(TItem item);
        bool CanAddItem(TItem item);
        bool RemoveItem(TItem item);
    }
}