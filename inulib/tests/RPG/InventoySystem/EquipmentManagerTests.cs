using System;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;
using INUlib.RPG.InventorySystem;
using INUlib.RPG.ItemSystem;

namespace Tests.Runtime.RPG.InventoySystem
{
    public class EquipmentManagerTests
    {
        #region Setup
        public class TestEquippable : IEquippableItem
        {
            public int ItemId => 0;
            public int InstanceId => 999;
            public int[] TargetSlotIds { get; private set; }
            public int[] DeactivateSlots { get; private set; }
            public override string ToString() => $"Target Slots: [{String.Join(", ", TargetSlotIds)}]";
            
            public TestEquippable(int slot)
            {
                TargetSlotIds = new[] { slot };
                DeactivateSlots = null;
            }

            public TestEquippable(int slot, int[] deactivateSlots)
            {
                TargetSlotIds = new[] { slot };
                DeactivateSlots = deactivateSlots;
            }

            public TestEquippable(int[] slots)
            {
                TargetSlotIds = slots;
                DeactivateSlots = null;
            }
        }
        
        private EquipmentsManager<TestEquippable> _manager;
        IEquipmentUser<TestEquippable> _user;
        private int _helmetSlot = 0;
        private int _armorSlot = 1;
        private int _weaponSlot = 2;
        private int _userItemEquippedCalled = 0;
        private int _userItemUnequippedCalled = 0;
        
        [SetUp]
        public void Setup()
        {
            _user = Substitute.For<IEquipmentUser<TestEquippable>>();
            _userItemEquippedCalled = 0;
            _userItemUnequippedCalled = 0;
            
            _user.When(x => x.OnItemEquipped(Arg.Any<TestEquippable>())).Do(x =>
            {
                _userItemEquippedCalled++;
            });
            _user.When(x => x.OnItemUnequipped(Arg.Any<TestEquippable>())).Do(x =>
            {
                _userItemUnequippedCalled++;
            });
            
            EquipmentSlot<TestEquippable> slot1 = new EquipmentSlot<TestEquippable>(_helmetSlot); 
            EquipmentSlot<TestEquippable> slot2 = new EquipmentSlot<TestEquippable>(_armorSlot);
            EquipmentSlot<TestEquippable> slot3 = new EquipmentSlot<TestEquippable>(_weaponSlot);
            List<EquipmentSlot<TestEquippable>> allSlots = new List<EquipmentSlot<TestEquippable>>()
            {
                slot1, slot2, slot3
            };
            
            _manager = new EquipmentsManager<TestEquippable>(allSlots, _user);
        }
        #endregion
        
        
        #region Tests
        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void CanItemBeEquippedAt_Returns_Cannot_Place_With_Non_IEquippable_Item(int equipSlotId)
        {
            IItemInstance itemInstance = Substitute.For<IItemInstance>();
            PlacementState equipState = _manager.CanItemBeEquippedAt(itemInstance, equipSlotId);
            Assert.AreEqual(PlacementState.CannotPlace, equipState);
        }

        [Test]
        [TestCase(0, PlacementState.CanPlace)]
        [TestCase(1, PlacementState.CanPlace)]
        [TestCase(2, PlacementState.CanPlace)]
        [TestCase(5, PlacementState.CannotPlace)]
        public void CanItemBeEquippedAt_Correctly_Tells_If_Item_Can_Be_Equipped_In_Existing_Free_Slot(int itemSlot, PlacementState expected)
        {
            TestEquippable itemInstance = new TestEquippable(itemSlot);
            PlacementState equipState = _manager.CanItemBeEquippedAt(itemInstance, itemSlot);
            Assert.AreEqual(expected, equipState);
        }
        
        [Test]
        public void Cant_AutoEquip_Item_That_Is_Not_An_IEquippableItem()
        {
            IItemInstance itemInstance = Substitute.For<IItemInstance>();
            bool wasEquipped = _manager.AutoEquipItem(itemInstance);
            Assert.IsFalse(wasEquipped); 
        }
        
        [Test]
        public void Cant_AutoEquip_Item_If_Equipments_Manager_Has_No_Slot_For_It()
        {
            TestEquippable itemInstance = new TestEquippable(10);
            bool wasEquipped = _manager.AutoEquipItem(itemInstance);
            Assert.IsFalse(wasEquipped);
        }
        
        [Test]
        [TestCase(0, 0, true, "Item should have been equipped")]
        [TestCase(1, 1, true, "Item should have been equipped")]
        [TestCase(2, 2, true, "Item should have been equipped")]
        [TestCase(55, 55, false, "Slot does not exist. Item should not have been equipped")]
        [TestCase(0, 3, false, "Item slot is correct but trying to equip in non existent slot should not work")]
        [TestCase(0, 1, false, "Item slot is correct but trying to equip in wrong slot should not work")]
        public void EquipItem_In_Slot_Works_Properly(int itemSlot, int equipInSlot, bool expected, string errorMsg)
        {
            TestEquippable itemInstance = new TestEquippable(itemSlot);
            TestEquippable equipped = _manager.EquipItem(itemInstance, equipInSlot);

            bool equippedIsItemInstance = itemInstance == equipped;
            Assert.AreEqual(expected, equippedIsItemInstance, errorMsg);
        }
        
        [Test]
        public void UnequipItem_Correctly_Unequips_Item()
        {
            TestEquippable itemInstance = new TestEquippable(_helmetSlot);
            _manager.EquipItem(itemInstance, _helmetSlot);
            var unequipped = _manager.UnequipItem(itemInstance);

            Assert.IsTrue(unequipped);
        }
        
        [Test]
        public void UnequipItem_Does_Nothing_If_Item_Is_Not_Equipped()
        {
            TestEquippable itemInstance = new TestEquippable(_helmetSlot);
            var unequipped = _manager.UnequipItem(itemInstance);

            Assert.IsFalse(unequipped);
        }
        
        [Test]
        public void UnequipItem_Does_Nothing_If_Item_Is_Not_IEquippable()
        {
            IItemInstance itemInstance = Substitute.For<IItemInstance>();
            var unequipped = _manager.UnequipItem(itemInstance);

            Assert.IsFalse(unequipped);
        }
        
        [Test]
        public void UnequipItemFrom_Works_Properly()
        {
            TestEquippable itemInstance = new TestEquippable(_helmetSlot);
            _manager.EquipItem(itemInstance, _helmetSlot);
            TestEquippable unequipped = _manager.UnequipItemFrom(_helmetSlot);
            
            Assert.AreSame(itemInstance, unequipped);
        }
        
        [Test]
        public void UnequipItemFrom_Does_Nothing_If_There_Is_No_Item_In_Slot()
        {
            var unequipped = _manager.UnequipItemFrom(_helmetSlot);
            Assert.IsNull(unequipped);
        }

        [Test]
        public void Auto_Equip_Correctly_Equips_Item_At_The_Correct_Slot()
        {
            TestEquippable itemInstance = new TestEquippable(_helmetSlot);
            bool wasEquipped = _manager.AutoEquipItem(itemInstance);
            Assert.IsTrue(wasEquipped);
        }

        [Test]
        public void Auto_Equip_Wont_Work_If_Slot_Already_Has_An_Item_Equipped()
        {
            TestEquippable itemInstance1 = new TestEquippable(_helmetSlot);
            TestEquippable itemInstance2 = new TestEquippable(_helmetSlot);
            _manager.AutoEquipItem(itemInstance1);
            bool wasEquipped = _manager.AutoEquipItem(itemInstance2);
            Assert.IsFalse(wasEquipped);
        }

        [Test]
        public void EquipItem_Properly_Replaces_Previously_Equipped_Item()
        {
            TestEquippable itemInstance1 = new TestEquippable(_helmetSlot);
            TestEquippable itemInstance2 = new TestEquippable(_helmetSlot);
            _manager.EquipItem(itemInstance1, _helmetSlot);
            
            TestEquippable result = _manager.EquipItem(itemInstance2, _helmetSlot);
            Assert.AreEqual(itemInstance1, result);
        }

        [Test]
        public void EquipItem_Properly_Replaces_Item_From_Disable_Slots()
        {
            TestEquippable itemInstance1 = new TestEquippable(_helmetSlot);
            TestEquippable itemInstance2 = new TestEquippable(_armorSlot, new [] { _helmetSlot });
            _manager.EquipItem(itemInstance1, _helmetSlot);
            
            TestEquippable result = _manager.EquipItem(itemInstance2, _armorSlot);
            Assert.AreEqual(itemInstance1, result);
        }

        [Test]
        [TestCase(0, true)]
        [TestCase(1, true)]
        [TestCase(2, true)]
        [TestCase(10, false)]
        public void HasSlot_by_Slot_Id_Function_Works_Properly(int slotId, bool expected)
        {
            bool hasSlot = _manager.HasSlot(slotId);
            Assert.AreEqual(expected, hasSlot);
        }
        
        [Test]
        [TestCase(0, true)]
        [TestCase(1, true)]
        [TestCase(2, true)]
        [TestCase(10, false)]
        public void HasSlotForItem_Works_Properly(int slotId, bool expected)
        {
            TestEquippable itemInstance = new TestEquippable(slotId);
            bool hasSlot = _manager.HasSlotForItem(itemInstance);
            Assert.AreEqual(expected, hasSlot);
        }
        
        [Test]
        public void HasSlotForItem_Returns_False_For_Item_That_Is_Not_IEquippable()
        {
            IItemInstance itemInstance = Substitute.For<IItemInstance>();
            bool hasSlot = _manager.HasSlotForItem(itemInstance);
            Assert.IsFalse(hasSlot);
        }

        [Test]
        public void OnItemEquipped_From_IEquipmentUser_Is_Called_With_AutoEquip_Item()
        {
            TestEquippable itemInstance = new TestEquippable(_helmetSlot);
            _manager.AutoEquipItem(itemInstance);
            
            Assert.AreEqual(1, _userItemEquippedCalled, "Equipment User OnItemEquipped should've been called exactly one time");
        }
        
        [Test]
        public void OnItemEquipped_From_IEquipmentUser_Is_Called_With_EquipItem()
        {
            TestEquippable itemInstance = new TestEquippable(_helmetSlot);
            _manager.EquipItem(itemInstance, _helmetSlot);
            
            Assert.AreEqual(1, _userItemEquippedCalled, "Equipment User OnItemEquipped should've been called exactly one time");
        }
        
        [Test]
        public void OnItemEquipped_From_IEquipmentUser_Is_Not_Called_If_Item_Was_Not_Equipped_With_AutoEquipItem()
        {
            IItemInstance itemInstance = Substitute.For<IItemInstance>();
            _manager.AutoEquipItem(itemInstance);
            
            Assert.AreEqual(0, _userItemEquippedCalled, "Equipment User OnItemEquipped should NOT have been called");
        }
        
        [Test]
        public void OnItemEquipped_From_IEquipmentUser_Is_Not_Called_If_Item_Was_Not_Equipped_With_EquipItem()
        {
            TestEquippable itemInstance = new TestEquippable(_helmetSlot);
            _manager.EquipItem(itemInstance, _armorSlot);
            
            Assert.AreEqual(0, _userItemEquippedCalled, "Equipment User OnItemEquipped should NOT have been called");
        }
        
        [Test]
        public void OnItemUnequipped_From_IEquipmentUser_Is_Called_With_Unequip_Item()
        {
            TestEquippable itemInstance = new TestEquippable(_helmetSlot);
            _manager.AutoEquipItem(itemInstance);
            _manager.UnequipItem(itemInstance);
            
            Assert.AreEqual(1, _userItemUnequippedCalled, "Equipment User OnItemEquipped should've been called exactly one time");
        }
        
        [Test]
        public void OnItemUnequipped_From_IEquipmentUser_Is_Not_Called_If_Item_Was_Not_Unequipped_With_UnequipItem()
        {
            TestEquippable equipped = new TestEquippable(_helmetSlot);
            IItemInstance notEquipped = Substitute.For<IItemInstance>();
            _manager.AutoEquipItem(equipped);
            _manager.UnequipItem(notEquipped);
            
            Assert.AreEqual(0, _userItemUnequippedCalled, "Equipment User OnItemEquipped should NOT have been called");
        }
        
        [Test]
        [TestCase(0)]
        [TestCase(10)]
        public void OnItemUnequipped_From_IEquipmentUser_Is_Not_Called_If_Item_Was_Not_Unequipped_With_UnequipItemFrom(int slotId)
        {
            _manager.UnequipItemFrom(slotId);
            Assert.AreEqual(0, _userItemUnequippedCalled, "Equipment User OnItemEquipped should NOT have been called");
        }

        [Test]
        public void Auto_Equip_Item_Wont_Work_If_There_Is_At_Least_One_DeactivateSlots_That_Is_Not_Empty()
        {
            TestEquippable deactivate = new TestEquippable(_helmetSlot, new []{ _armorSlot });
            TestEquippable item = new TestEquippable(_armorSlot, null);

            _manager.EquipItem(item, _armorSlot);
            var equipped = _manager.AutoEquipItem(deactivate);
            
            Assert.IsFalse(equipped, "Item should not have been equipped");
        }
        
        [Test]
        public void Cant_Equip_Item_If_More_Than_One_DeactivateSlots_Is_Not_Empty()
        {
            TestEquippable deactivate = new TestEquippable(_helmetSlot, new []{ _armorSlot, _weaponSlot });
            TestEquippable armor = new TestEquippable(_armorSlot, null);
            TestEquippable weapon = new TestEquippable(_weaponSlot, null);

            _manager.EquipItem(armor, _armorSlot);
            _manager.EquipItem(weapon, _weaponSlot);
            
            var equipped = _manager.EquipItem(deactivate, _helmetSlot);
            Assert.IsNull(equipped, "Item should not have been equipped");
        }

        [Test]
        public void Cant_Equip_Item_If_Item_Slot_Is_Not_Free_And_One_DeactivateSlots_Is_Not_Empty()
        {
            TestEquippable deactivate = new TestEquippable(_helmetSlot, new []{ _armorSlot });
            TestEquippable armor = new TestEquippable(_armorSlot, null);
            TestEquippable helmet = new TestEquippable(_helmetSlot, null);

            _manager.EquipItem(armor, _armorSlot);
            _manager.EquipItem(helmet, _helmetSlot);
            
            var equipped = _manager.EquipItem(deactivate, _helmetSlot);
            Assert.IsNull(equipped);
        }

        [Test]
        public void Can_Equip_Item_If_Only_One_DeactivateSlots_Is_Not_Empty_And_Item_Slot_Is_Free()
        {
            TestEquippable deactivate = new TestEquippable(_helmetSlot, new []{ _armorSlot });
            TestEquippable item = new TestEquippable(_armorSlot, null);

            _manager.EquipItem(item, _armorSlot);
            var equipped = _manager.EquipItem(deactivate, _helmetSlot);
            
            Assert.AreEqual(item, equipped, "Return should have been the same as the deactivated item slot");
        }

        [Test]
        public void Auto_Equip_Item_With_DeactivateSlots_With_Same_Value_As_EquipSlotId_Has_No_Effect()
        {
            TestEquippable deactivate = new TestEquippable(_helmetSlot, new []{ _helmetSlot });
            bool equipped = _manager.AutoEquipItem(deactivate);
            
            Assert.IsTrue(equipped, "Item should have been equipped");
            Assert.IsTrue(_manager.IsSlotActive(_helmetSlot), "Equipped item slot should be active");
        }
        
        [Test]
        public void Equip_Item_With_DeactivateSlots_With_Same_Value_As_EquipSlotId_Has_No_Effect()
        {
            TestEquippable deactivate = new TestEquippable(_helmetSlot, new []{ _helmetSlot });
            var equipped = _manager.EquipItem(deactivate, _helmetSlot);
            
            Assert.AreEqual(deactivate, equipped, "Item should have been equipped");
            Assert.IsTrue(_manager.IsSlotActive(_helmetSlot), "Equipped item slot should be active");
        }
        
        [Test]
        public void AutoEquipping_Item_With_DeactivateSlots_Will_Properly_Deactivate_Them()
        {
            TestEquippable deactivate = new TestEquippable(_helmetSlot, new []{ _armorSlot, _weaponSlot });
            _manager.AutoEquipItem(deactivate);

            Assert.IsFalse(_manager.IsSlotActive(_armorSlot), "Armor slot example should be deactivated");
            Assert.IsFalse(_manager.IsSlotActive(_weaponSlot), "Weapon slot example should be deactivated");
        }
        
        [Test]
        public void Equipping_Item_With_DeactivateSlots_Will_Properly_Deactivate_Them()
        {
            TestEquippable deactivate = new TestEquippable(_helmetSlot, new []{ _armorSlot, _weaponSlot });
            _manager.EquipItem(deactivate, _helmetSlot);
            
            Assert.IsFalse(_manager.IsSlotActive(_armorSlot), "Armor slot example should be deactivated");
            Assert.IsFalse(_manager.IsSlotActive(_weaponSlot), "Weapon slot example should be deactivated");
        }
        
        [Test]
        public void Unequipping_Item_With_DeactivateSlots_Will_Properly_Reactivate_Them()
        {
            TestEquippable deactivate = new TestEquippable(_helmetSlot, new []{ _armorSlot, _weaponSlot });
            _manager.AutoEquipItem(deactivate);
            _manager.UnequipItem(deactivate);
            
            Assert.IsTrue(_manager.IsSlotActive(_armorSlot), "Armor slot example should be activate");
            Assert.IsTrue(_manager.IsSlotActive(_weaponSlot), "Weapon slot example should be activate");           
        }
        
        [Test]
        public void Unequip_Item_From_With_DeactivateSlots_Will_Properly_Reactivate_Them()
        {
            TestEquippable deactivate = new TestEquippable(_helmetSlot, new []{ _armorSlot, _weaponSlot });
            _manager.AutoEquipItem(deactivate);
            _manager.UnequipItemFrom(_helmetSlot);
            
            Assert.IsTrue(_manager.IsSlotActive(_armorSlot), "Armor slot example should be activate");
            Assert.IsTrue(_manager.IsSlotActive(_weaponSlot), "Weapon slot example should be activate");           
        }

        [Test]
        public void Can_AutoEquip_Item_Which_Has_Multiple_Valid_Slots_If_At_Least_One_Of_Them_Is_Free()
        {
            TestEquippable auto = new TestEquippable(new []{ _helmetSlot, _armorSlot, _weaponSlot });
            TestEquippable armor = new TestEquippable(_armorSlot);
            TestEquippable helmet = new TestEquippable(_helmetSlot);

            _manager.AutoEquipItem(armor);
            _manager.AutoEquipItem(helmet);

            bool equipped = _manager.AutoEquipItem(auto);
            Assert.IsTrue(equipped, "Item should have been automatically equipped");
        }

        [Test]
        public void Can_Equip_Item_In_Any_Of_The_Valid_Slots_For_It()
        {
            TestEquippable equip = new TestEquippable(new []{ _helmetSlot, _armorSlot, _weaponSlot });

            Assert.AreEqual(equip, _manager.EquipItem(equip, _helmetSlot));
            _manager.UnequipItem(equip);
            
            Assert.AreEqual(equip, _manager.EquipItem(equip, _armorSlot));
            _manager.UnequipItem(equip);
            
            Assert.AreEqual(equip, _manager.EquipItem(equip, _weaponSlot));
        }
        #endregion
    }
}