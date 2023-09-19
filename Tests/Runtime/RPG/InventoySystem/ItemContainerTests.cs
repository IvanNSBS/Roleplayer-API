using NUnit.Framework;
using INUlib.RPG.InventorySystem;
using INUlib.RPG.ItemSystem;
using Utils.Math;

namespace Tests.Runtime.RPG.InventoySystem
{
    public class ItemContainerTests
    {
        #region Setups
        private class TestMetaItem : IMetaItem<TestItemInstance>
        {
            public int Id { get; set; }
            public int SlotType { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }

            public string GetFullName() => "Test Item Name";

            public string GetDescription() => "Test Item Description";

            public TestMetaItem(int width = 1, int height = 1)
            {
                Width = width;
                Height = height;
                Id = 0;
                SlotType = 0;
            }

            public TestItemInstance CreateInstance() => new TestItemInstance();
        }

        private class TestItemInstance : IItemInstance
        {
            public int ItemId { get; }
            public int InstanceId { get; }
        }

        private ItemContainer<TestItemInstance> _container;
        private TestMetaItem _metaItemFactory;
        private int _width = 16;
        private int _height = 16;
        private float _celSize = 1;
        
        [SetUp]
        public void Setup()
        {
            _container = new ItemContainer<TestItemInstance>(_width, _height, _celSize);
            _metaItemFactory = new TestMetaItem();
        }
        #endregion
        
        
        #region Tests
        [Test]
        public void Item_Is_Correctly_Placed_When_There_Is_No_Overlapping_Item()
        {
            TestItemInstance i1 = _metaItemFactory.CreateInstance();
            var @switch = _container.PlaceItemAt(i1, new int2(0, 0));   

            Assert.IsNull(@switch);
        }

        public void Will_Replace_Item_When_Overlapping()
        {
            TestItemInstance i1 = _metaItemFactory.CreateInstance();
            TestItemInstance i2 = _metaItemFactory.CreateInstance();

            _container.PlaceItemAt(i1, new int2(0, 0));
        }
        #endregion
    }
}