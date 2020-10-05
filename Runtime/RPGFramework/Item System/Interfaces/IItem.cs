using UnityEngine;

namespace RPGFramework.ItemSystem
{
    public interface IItem
    {
        Vector2Int ItemSize { get; set; }
        Sprite Sprite { get; set; }
    }
}