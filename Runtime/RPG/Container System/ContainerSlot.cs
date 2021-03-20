using RPG.ItemSystem;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.ContainerSystem
{
    public class ContainerSlot<TItem> where TItem : IItem
    {
        #region Fields

        private IItem m_slotItem;
        public Vector2Int m_coordinates;
        public Sprite m_slotSprite;
        public Image m_Image;
        #endregion Fields
        
        #region Constructors
        public ContainerSlot(int x, int y, Transform parent, Sprite slotSprite = null, IItem item = null)
        {
            m_slotItem = item;
            m_slotSprite = slotSprite;
            m_coordinates = new Vector2Int(x,y);
            
            GameObject go = new GameObject($"Slot ({x},{y})");
            go.transform.SetParent(parent);
            var img = go.AddComponent<Image>();

            m_Image = img;
        }
        #endregion Constructors
    }
}