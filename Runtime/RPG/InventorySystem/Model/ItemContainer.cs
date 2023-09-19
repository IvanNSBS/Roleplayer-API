using Utils.Math;
using System.Linq;
using INUlib.Gameplay.Grids;
using System.Collections.Generic;
using INUlib.RPG.ItemSystem;

namespace INUlib.RPG.InventorySystem
{
    public class ItemContainer<T> where T : class, IItemInstance
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
        /// <summary>
        /// Checks if an item can be placed at the given coordinate
        /// </summary>
        /// <param name="item"></param>
        /// <param name="slotCoord"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Will attempt to place an item at the given coordinate.
        /// </summary>
        /// <param name="item">The item to be placed</param>
        /// <param name="slotCoord">The container coordinate to place it</param>
        /// <returns>
        /// Returns the item that was added if the item was added succesfully
        /// If there was only one item in the same coordinate, the items
        /// will be switched and the function will return the item that was replaced.
        /// If the item was not placed because the coordinate is invalid or because
        /// the new item will occupy the slots of more than one item it will
        /// return null 
        /// </returns>
        public T PlaceItemAt(T item, int2 slotCoord)
        {
            PlacementState placement = CanPlaceItemAt(item, slotCoord);
            bool canPlace = placement != PlacementState.CannotPlace;
            if (!canPlace)
                return null;

            T replaced = item;
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

        public bool FindSlotForItemAndPlaceIt()
        {
            return false;
        }
        
        /// <summary>
        /// Gets the item that is at the given coordinate
        /// </summary>
        /// <param name="slotCoord">The coordinate to check the item</param>
        /// <returns>
        /// The item at the given coordinate if there's one item there.
        /// Null if the coordinate is invalid or there is no item in the
        /// given slot.
        /// </returns>
        public T GetItemAt(int2 slotCoord) => _itemSlots.GetItemAt(slotCoord);

        /// <summary>
        /// Removes an item from the container using the item reference.
        /// </summary>
        /// <param name="item">The item to be removed</param>
        /// <returns>
        /// True if the item was in the inventory
        /// False otherwise
        /// </returns>
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

        /// <summary>
        /// Removes an item that is occupying a certain slot position
        /// </summary>
        /// <param name="slotCoord">The slot to remove the item</param>
        /// <returns>
        /// The item that was removed or null if there was no item
        /// in the coordinate
        /// </returns>
        public T RemoveItemAt(int2 slotCoord)
        {
            T item = GetItemAt(slotCoord);
            RemoveItem(item);

            return item;
        }

        /// <summary>
        /// Checks if the given item is in the container, given the item Id
        /// </summary>
        /// <param name="item">The item to be checked</param>
        /// <returns>
        /// The list of coordinates that this item is occupying in the container
        /// or null if there's no such item.
        /// </returns>
        public List<int2> ItemIsInContainer(T item) => ItemIsInContainer(item.ItemId);

        /// <summary>
        /// Checks if the given item is in the container, given the item Id
        /// </summary>
        /// <param name="itemId">The item Id</param>
        /// <returns>
        /// The list of coordinates that this item is occupying in the container
        /// or null if there's no such item.
        /// </returns>
        public List<int2> ItemIsInContainer(int itemId)
        {
            int id = itemId;
            T item = _itemsInside.Find(x => x.ItemId == id);
            if (item == null)
                return null;

            return _slotsOccupiedByItem[item];
        }
        #endregion
        
        
        #region Helper Methods
        private List<int2> GetAllSlotsOccupiedFromItem(T item)
        {
            if (!HasTheItemInside(item))
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
        
        private bool HasTheItemInside(T item) => _itemsInside.Contains(item);
        #endregion
    }
}