using System;
using System.Collections;
using System.Collections.Generic;
using RPGFramework.ItemSystem;
using UnityEngine;
using UnityEngine.UI;


namespace RPGFramework.ContainerSystem
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

        private Image backgroundImage;
        private GridLayoutGroup gridLayout;
        private ContainerGrid<IItem> _grid;
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
            _grid = new ContainerGrid<IItem>(containerSize, slotSize, transform);
                
            backgroundImage = GetComponent<Image>();
            backgroundImage.rectTransform.sizeDelta = _grid.GridBounds;
            gridLayout = GetComponent<GridLayoutGroup>();
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = containerSize.x;
            gridLayout.cellSize = new Vector2(slotSize, slotSize);
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
            Debug.Log(_grid.MouseToLocal());
            var slot = _grid.LocalPosToGrid(_grid.MouseToLocal());
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