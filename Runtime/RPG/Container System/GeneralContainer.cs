using UnityEngine;
using UnityEngine.UI;
using Essentials.Grids;
using System.Collections.Generic;
using System;
using RPG.ItemSystem;

namespace RPG.ContainerSystem
{
    // Disable field never assigned to warnings
    #pragma warning disable 0649
    
    /// <summary>
    /// Implementation of a general container that
    /// can hold any type of items
    /// </summary>
    [RequireComponent(typeof(Image), typeof(GridLayoutGroup))]
    public class GeneralContainer : MonoBehaviour, IContainer<IItem>
    {
        #region Fields
        [SerializeField] private Sprite containerSlotSprite;
        [SerializeField] private Vector2Int containerSize = new Vector2Int(1,1);
        [SerializeField] private float slotSize = 1;
        [SerializeField] private List<Type> types;

        private Image m_backgroundImage;
        private GridLayoutGroup m_gridLayout;
        private SquareGrid<ContainerSlot<IItem>> m_squareGrid;
        #endregion Fields
        
        #region Properties
        #endregion Properties
        
        
        #region ContainerProperties
        public List<IItem> ItemsInside { get; set; }
        public Vector2Int ContainerSize { get => containerSize; set => containerSize = value; }
        public ContainerGrid<IItem> ContainerGrid { get; set; }
        #endregion ContainerProperties

        
        #region MonoBehaviour Methods
        public void Awake()
        {
            m_squareGrid = new ContainerGrid<IItem>(containerSize, slotSize, transform);
                
            m_backgroundImage = GetComponent<Image>();
            m_backgroundImage.rectTransform.sizeDelta = m_squareGrid.GridBounds;
            m_gridLayout = GetComponent<GridLayoutGroup>();
            m_gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            m_gridLayout.constraintCount = containerSize.x;
            m_gridLayout.cellSize = new Vector2(slotSize, slotSize);
        }

        // private void OnValidate()
        // {
        //     for (int i = 0; i < transform.childCount; i++)
        //     {
        //         StartCoroutine(Destroy(transform.GetChild(i).gameObject));
        //     }
        //     Awake();
        // }
        //
        // IEnumerator Destroy(GameObject go)
        // {
        //     yield return new WaitForEndOfFrame();
        //     DestroyImmediate(go);
        // }

        private void Update()
        {
            for (int x = 0; x < m_squareGrid.GridSize.x; x++)
            {
                for (int y = 0; y < m_squareGrid.GridSize.y; y++)
                {
                    var curSlot = m_squareGrid.GridItems[x, y].m_Image.color = Color.white;
                }
            }
            var slot = m_squareGrid.LocalPosToGrid(m_squareGrid.WorldPosToLocal(Input.mousePosition));
            if(slot != null)
                slot.m_Image.color = Color.green;
        }

        #endregion MonoBehaviour Methods
        
        
        #region Container Methods

        public void AddItem(IItem gridCel)
        {
            
        }

        public bool CanAddItem(IItem gridCel)
        {
            return false;
        }

        public bool RemoveItem(IItem gridCel)
        {
            // Do additional processing
            return ItemsInside.Remove(gridCel);
        }

        #endregion Container Methods
    }
}