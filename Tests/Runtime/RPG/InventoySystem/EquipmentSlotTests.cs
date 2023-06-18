using NSubstitute;
using NUnit.Framework;
using INUlib.RPG.InventorySystem;

namespace Tests.Runtime.RPG.InventoySystem
{
    public class EquipmentSlotTests
    {
        #region Setup
        private EquipmentSlot<IEquippableItem> _equipmentSlot;
        private int _acceptsId = 33;
        
        [SetUp]
        public void Setup()
        {
            _equipmentSlot = new EquipmentSlot<IEquippableItem>(_acceptsId);
        }
        #endregion
        
        
        #region Tests

        [Test]
        [TestCase(0, false)]
        [TestCase(5, false)]
        [TestCase(33, true)]
        public void Correctly_Runs_AcceptsItemType(int slotType, bool expected)
        {
            bool value = _equipmentSlot.AcceptsItemType(slotType);
            Assert.AreEqual(expected, value);
        }
        
        [Test]
        [TestCase(new int[]{ 0, 1, 2 }, false)]
        [TestCase(new int[]{ 0, 1, 33 }, true)]
        [TestCase(new int[]{ 33 }, true)]
        public void Correctly_Runs_AcceptsItemType_With_Id_Array(int[] slotType, bool expected)
        {
            bool value = _equipmentSlot.AcceptsItemType(slotType);
            Assert.AreEqual(expected, value);
        }

        [Test]
        public void Returns_Same_Item_When_Equipping_Item_Without_Anything_To_Replace()
        {
            IEquippableItem item = Substitute.For<IEquippableItem>();
            item.TargetSlotIds.Returns(new []{ _acceptsId} );
            
            var equipped = _equipmentSlot.EquipItem(item);
            Assert.AreSame(equipped, item);
        }
        
        [Test]
        public void Get_Equipped_Item_Returns_Null_If_There_Is_Nothing_Inside()
        {
            Assert.IsFalse(_equipmentSlot.HasItemEquipped(), "HasItemEquipped should be false");
            Assert.IsNull(_equipmentSlot.GetEquippedItem(), "GetEquippedItem should've been null");
        }

        [Test]
        public void Equip_Item_Is_Successfull_When_Has_The_Same_Slot_Type()
        {
            IEquippableItem item = Substitute.For<IEquippableItem>();
            item.TargetSlotIds.Returns(new []{_acceptsId});
            
            _equipmentSlot.EquipItem(item);
            
            Assert.IsTrue(_equipmentSlot.HasItemEquipped(), "HasItemEquipped should be true");
            Assert.AreEqual(item, _equipmentSlot.GetEquippedItem(), "GetEquippedItem should have been the same as item");
        }
        
        [Test]
        public void Equip_Item_Is_Unuccessfull_When_Slot_Type_Is_Different()
        {
            IEquippableItem item = Substitute.For<IEquippableItem>();
            item.TargetSlotIds.Returns(new []{ 999 });
            
            _equipmentSlot.EquipItem(item);
            
            Assert.IsFalse(_equipmentSlot.HasItemEquipped(), "HasItemEquipped should be false");
            Assert.IsNull(_equipmentSlot.GetEquippedItem(), "GetEquippedItem should've been null");
        }
        
        [Test]
        public void Get_Equipped_Item_Returns_Null_After_Equipping_And_Removing_An_Item()
        {
            IEquippableItem item = Substitute.For<IEquippableItem>();
            item.TargetSlotIds.Returns(new []{ _acceptsId });
            
            _equipmentSlot.EquipItem(item);
            _equipmentSlot.UnequipItem();
            
            Assert.IsFalse(_equipmentSlot.HasItemEquipped(), "HasItemEquipped should be false");
            Assert.IsNull(_equipmentSlot.GetEquippedItem(), "GetEquippedItem should've been null");
        }
        
        [Test]
        public void Returns_Replaced_Item_When_Equipping_Item_With_Something_Inside()
        {
            IEquippableItem item1 = Substitute.For<IEquippableItem>();
            IEquippableItem item2 = Substitute.For<IEquippableItem>();
            item2.TargetSlotIds.Returns(new []{ _acceptsId });
            item1.TargetSlotIds.Returns(new []{ _acceptsId });

            _equipmentSlot.EquipItem(item1);
            var old = _equipmentSlot.EquipItem(item2);
            Assert.AreEqual(item1, old, "Should've returned item1");
        }

        [Test]
        public void Cant_Equip_Item_In_Deactivated_Slot()
        {
            IEquippableItem item = Substitute.For<IEquippableItem>();
            item.TargetSlotIds.Returns(new []{ _acceptsId });

            _equipmentSlot.TryDeactivate();
            Assert.IsNull(_equipmentSlot.EquipItem(item), "Item shouldn't be equipped with disabled slot");
        }
        
        [Test]
        public void Cant_Deactivate_Slot_With_Item_Inside()
        {
            IEquippableItem item = Substitute.For<IEquippableItem>();
            item.TargetSlotIds.Returns(new []{ _acceptsId });
            _equipmentSlot.EquipItem(item);
            
            bool deactivated = _equipmentSlot.TryDeactivate();
            Assert.IsFalse(deactivated, "Slot should not be deactivated with item inside");
            Assert.IsTrue(_equipmentSlot.IsSlotActive(), "IsSlotActive should be true");
        }

        [Test]
        public void Slot_Can_Be_Reactivated()
        {
            IEquippableItem item = Substitute.For<IEquippableItem>();
            item.TargetSlotIds.Returns(new []{ _acceptsId });

            _equipmentSlot.TryDeactivate();
            Assert.IsNull(_equipmentSlot.EquipItem(item), "Item shouldn't be equipped with disabled slot");
            
            _equipmentSlot.Activate();
            Assert.IsTrue(_equipmentSlot.IsSlotActive(), "Slot should have been active");
            Assert.IsNotNull(_equipmentSlot.EquipItem(item), "Item should be equipped since slot was activated");
        }
        #endregion
    }
}