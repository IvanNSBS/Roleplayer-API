using UnityEngine;

namespace RPG.ItemSystem
{
    public interface IItem
    {
        Vector2Int ItemSize { get; set; }
        Sprite Sprite { get; set; }
    }
}