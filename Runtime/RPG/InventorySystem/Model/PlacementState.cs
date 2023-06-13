namespace INUlib.RPG.InventorySystem
{
    public enum PlacementState
    {
        /// <summary>
        /// The item can freely be placed
        /// </summary>
        CanPlace,
        
        /// <summary>
        /// The item cannot be placed because it will overlap
        /// with more than one item
        /// </summary>
        CannotPlace,
        
        /// <summary>
        /// The item can be placed, but it will replace a certain
        /// item in the inventory
        /// </summary>
        WillReplace
    }
}