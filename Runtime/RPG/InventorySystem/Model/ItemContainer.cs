using System.Collections.Generic;
using System.Linq;
using INUlib.Gameplay.Grids;
using Utils.Math;

namespace INUlib.RPG.InventorySystem
{
    public enum PlacementState
    {
        CanPlace,
        CannotPlace,
        WillReplace
    }
    
    public class ItemContainer<T> where T : class, IItem
    {
        #region Fields
        private List<T> _itemsInside;
        private Dictionary<T, List<int2>> _slotsOccupiedByItem;

        private Grid2d<T> _itemSlots;
        
        private int2 _size;
        private float _slotSize;
        #endregion
        
        #region Properties
        public int2 Size => _size;
        public float SlotSize => _slotSize;
        #endregion

        #region Constructors
        public ItemContainer(int width, int height, float slotSize)
        {
            _itemSlots = new Grid2d<T>(width, height, slotSize, new float3(0,0,0));
            _itemsInside = new List<T>();
            _slotsOccupiedByItem = new Dictionary<T, List<int2>>();
            
            _size = new int2(width, height);
            _slotSize = slotSize;
        }
        #endregion


        #region Methods
        public PlacementState CanPlaceItemAt(T item, int2 slotCoord)
        {
            bool canPlace = true;
            int itemQuantity = 0;
            HashSet<T> itemsInArea = new HashSet<T>();
            
            for (int i = 0; i < slotCoord.x; i++)
            {
                for (int j = 0; j < slotCoord.y; j++)
                {
                    int2 coord = new int2(i, j);
                    
                    T itemAt = _itemSlots.GetItemAt(coord);
                    if (itemAt != null)
                        itemsInArea.Add(itemAt);
                    
                    canPlace &= _itemSlots.IsCellFree(coord);
                }
            }

            if (itemsInArea.Count == 0)
                return PlacementState.CanPlace;
            if (itemsInArea.Count == 1)
                return PlacementState.WillReplace;
            
            return PlacementState.WillReplace;
        }

        public T PlaceItemAt(T item, int2 slotCoord)
        {
            PlacementState placement = CanPlaceItemAt(item, slotCoord);
            bool canPlace = placement != PlacementState.CannotPlace;
            if (!canPlace)
                return null;

            T replaced = null;
            List<T> willBeReplaced = GetOverlappingItems(slotCoord, item);
            if (willBeReplaced.Count == 1)
            {
                replaced = willBeReplaced[0];
                RemoveItem(replaced);
            }
            
            List<int2> itemSlots = new List<int2>();
            for (int i = 0; i < slotCoord.x; i++)
            {
                for (int j = 0; j < slotCoord.y; j++)
                {
                    int2 slot = new int2(i, j);
                    _itemSlots.SetCellItem(new int2(i, j), item);
                    
                    itemSlots.Add(slot);
                }
            }
            
            _itemsInside.Add(item);
            _slotsOccupiedByItem.Add(item, itemSlots);
            return replaced;
        }
        
        public T GetItemAt(int2 slotCoord) => _itemSlots.GetItemAt(slotCoord);

        public bool RemoveItem(T item)
        {
            var slots = GetAllSlotsOccupiedFromItem(item);
            if (slots == null)
                return false;
            
            foreach (int2 slot in slots)
                _itemSlots.SetCellItem(slot, null);

            _slotsOccupiedByItem.Remove(item);
            _itemsInside.Remove(item);
            return true;
        }

        public T RemoveItemAt(int2 slotCoord)
        {
            T item = GetItemAt(slotCoord);
            RemoveItem(item);

            return item;
        }
        #endregion
        
        
        #region Helper Methods
        private List<int2> GetAllSlotsOccupiedFromItem(T item)
        {
            if (!ItemIsInContainer(item))
                return null;

            return _slotsOccupiedByItem[item];
        }

        private List<T> GetOverlappingItems(int2 slotCoord, T newItem)
        {
            HashSet<T> itemsInArea = new HashSet<T>();
            
            for (int i = 0; i < slotCoord.x; i++)
            {
                for (int j = 0; j < slotCoord.y; j++)
                {
                    int2 coord = new int2(i, j);
                    
                    T itemAt = _itemSlots.GetItemAt(coord);
                    if (itemAt != null)
                        itemsInArea.Add(itemAt);
                }
            }

            return itemsInArea.ToList();
        }
        
        private bool ItemIsInContainer(T item) => _itemsInside.Contains(item);
        #endregion
    }
}