using INUlib.RPG.CharacterSheet;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Runtime.RPG.Attributes
{
    public class RPGAttributesTests
    {
        #region Setup
        private class MockAttribute : IntAttribute
        {
            public MockAttribute(int defaultVal) : base(defaultVal) { } 
            public MockAttribute(int defaultVal, int maxVal) : base(defaultVal, maxVal) { }

            protected override int CalculateModifiers()
            {
                int flatSum = 0, pctSum = 0;
                _flatMods.ForEach(x => flatSum += x.Value);
                _percentMods.ForEach(x => pctSum += x.Value);

                return flatSum + pctSum;
            }
        }

        private int _dfValue = 1;
        private int _maxValue = 99;
        private MockAttribute _attr;
        bool _changed;

        [SetUp]
        public void Setup()
        {
            _changed = false;
            _attr = new MockAttribute(_dfValue, _maxValue);
            _attr.onValueChanged += x => _changed = true;
            _attr.onModifiersChanged += x => _changed = true;
        }
        #endregion


        #region Tests
        [Test]
        public void Attribute_Is_Correctly_Initialized()
        {
            Assert.IsTrue(_attr.defaultVal.CompareTo(_dfValue) == 0);
            Assert.IsTrue(_attr.maxVal.CompareTo(_maxValue) == 0);
        }

        [Test]
        [TestCase(99)]
        [TestCase(100)]
        [TestCase(99999)]
        [TestCase(54654)]
        public void Attribute_Wont_Inscrease_Past_Max_Value(int increaseAmnt)
        {
            _attr.Increase(increaseAmnt);
            Assert.IsTrue(_attr.Value == _maxValue);
        }

        [Test]
        [TestCase(5)]
        [TestCase(3)]
        [TestCase(2)]
        [TestCase(1)]
        [TestCase(38)]
        [TestCase(124)]
        [TestCase(999)]
        public void Attribute_Will_Always_Increase_Without_Max_Value(int increaseAmnt)
        {
            _attr = new MockAttribute(_dfValue);
            _attr.Increase(increaseAmnt);
            Assert.IsTrue(_attr.Value == increaseAmnt + _dfValue);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(10)]
        [TestCase(20)]
        [TestCase(100)]
        public void Attribute_Wont_Decrease_Beyond_Default_Value(int decreaseAmnt)
        {
            _attr.Decrease(decreaseAmnt);
            Assert.IsTrue(_attr.Value == _dfValue);
        }

        
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Attribute_Correctly_Calls_On_Value_Changed(bool increase)
        {
            if(increase)
                _attr.Increase(1);
            else
                _attr.Decrease(1);
                
            Assert.IsTrue(_changed);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(10)]
        public void Attribute_Correctly_Adds_Flat_Modifier(int addAmount)
        {
            for(int i = 0; i < addAmount; i++){
                _attr.AddFlatModifier(i+1);
                Assert.IsTrue(_attr.FlatMods[i].Value == i + 1);
            }

            Assert.IsTrue(_attr.FlatMods.Count == addAmount);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void Attribute_Correctly_Adds_Percent_Modifier(float addAmount)
        {
            for(int i = 0; i < addAmount; i++)
            {
                _attr.AddPercentModifier(i+1);
                Assert.IsTrue(_attr.PercentMods[i].Value == _dfValue * (i+1));
            }

            Assert.IsTrue(_attr.PercentMods.Count == addAmount);
        }


        [Test]
        public void Attribute_Correctly_Removes_Flat_Modifier()
        {
            var mod = _attr.AddFlatModifier(5);
            _attr.RemoveFlatModifier(mod);

            Assert.IsTrue(_attr.FlatMods.Count == 0 && _attr.Modifiers == 0);
        }

        [Test]
        public void Attribute_Correctly_Removes_Percent_Modifier()
        {
            var mod = _attr.AddPercentModifier(1);
            _attr.RemovePercentModifier(mod);

            Assert.IsTrue(_attr.PercentMods.Count == 0 && _attr.Modifiers == 0);
        }

        [Test]
        public void Attribute_Wont_Remove_Inexistent_Flat_Modifier()
        {
            IAttributeModifier<int> mod = Substitute.For<IAttributeModifier<int>>();
            Assert.IsFalse(_attr.RemoveFlatModifier(mod));
            Assert.IsTrue(_attr.FlatMods.Count == 0 && _attr.Modifiers == 0);
        }


        [Test]
        public void Attribute_Wont_Remove_Inexistent_Percent_Modifier()
        {
            IAttributeModifier<int> mod = Substitute.For<IAttributeModifier<int>>();
            Assert.IsFalse(_attr.RemovePercentModifier(mod));
            Assert.IsTrue(_attr.PercentMods.Count == 0 && _attr.Modifiers == 0);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void Attribute_Correctly_Calls_On_Modifiers_Changed(bool addFlat)
        {
            if(addFlat)
                _attr.AddFlatModifier(1);
            else
                _attr.AddPercentModifier(1);

            Assert.IsTrue(_changed);
        }

        [Test]
        [TestCase(5, 10, 1.0f, 15)]
        [TestCase(2,  1, 1.0f,  3)]
        [TestCase(1,  5, 0.0f,  5)]
        [TestCase(3,  1, 1.5f,  5)]
        public void Attribute_Correctly_Updates_Modifier_Field_With_Calculate_Modifiers_Method(int dfValue, int flat, float pct, int expected)
        {
            MockAttribute attr = new MockAttribute(dfValue);
            attr.AddFlatModifier(flat);
            attr.AddPercentModifier(pct);
            
            Assert.IsTrue(attr.Modifiers == expected);
            Assert.IsTrue(attr.Total == expected + dfValue);
        }

        [Test]
        public void Attribute_Gets_Correctly_Reset()
        {
            _attr.Increase(5);
            _attr.AddFlatModifier(20);
            _attr.AddPercentModifier(2);
            
            _attr.Reset();
            Assert.IsTrue(
                _attr.Modifiers == 0 && _attr.Value == _dfValue && 
                _attr.FlatMods.Count == 0 && _attr.PercentMods.Count == 0
            );
        }

        [Test]
        [TestCase(5, 4)]
        [TestCase(1, 2)]
        [TestCase(2, 4)]
        [TestCase(1, 3)]
        [TestCase(23, 9)]
        public void Int_Attribute_Correctly_Implements_Sum(int dfValue, int increase)
        {
            var at = new IntAttribute(dfValue);
            at.Increase(increase);

            Assert.IsTrue(at.Value == dfValue + increase);
        }

        [Test]
        [TestCase(5, 1)]
        [TestCase(9, 2)]
        [TestCase(12, 4)]
        [TestCase(2, 1)]
        [TestCase(23, 9)]
        public void Int_Attribute_Correctly_Implements_Subtraction(int dfValue, int dc)
        {
            var at = new IntAttribute(0);
            at.Increase(dfValue);
            at.Decrease(dc);
            Assert.IsTrue(at.Value == dfValue - dc);
        }

        [Test]
        [TestCase(5, 1.0f, 5)]
        [TestCase(2, 1.0f, 2)]
        [TestCase(1, 0.5f, 0)]
        [TestCase(5, 0.5f, 2)]
        public void Int_Attribute_Correctly_Implements_Scaling(int df, float factor, int expected)
        {
            var at = new IntAttribute(df);
            var scaled = at.AddPercentModifier(factor).Value;

            Assert.IsTrue(scaled == expected);
        }

        [Test]
        public void Int_Attribute_Correctly_Implements_Clamping()
        {
            var at = new IntAttribute(5, 10);
            at.Decrease(1);

            Assert.IsTrue(at.Value == 5);
            at.Increase(99);
            Assert.IsTrue(at.Value == 10);
        }

        [Test]
        [TestCase(5, 4)]
        [TestCase(1, 2)]
        [TestCase(2, 4)]
        [TestCase(1, 3)]
        [TestCase(23, 9)]
        public void Float_Attribute_Correctly_Implements_Sum(float dfValue, float increase)
        {
            var at = new FloatAttribute(dfValue);
            at.Increase(increase);

            Assert.IsTrue(at.Value == dfValue + increase);
        }

        [Test]
        [TestCase(5, 1)]
        [TestCase(9, 2)]
        [TestCase(12, 4)]
        [TestCase(2, 1)]
        [TestCase(23, 9)]
        public void Float_Attribute_Correctly_Implements_Subtraction(float dfValue, float dc)
        {
            var at = new FloatAttribute(0);
            at.Increase(dfValue);
            at.Decrease(dc);

            Assert.IsTrue(at.Value == dfValue - dc);
        }

        [Test]
        [TestCase(5.0f, 2.0f)]
        [TestCase(1.0f, 0.2f)]
        [TestCase(2, 0.98f)]
        [TestCase(1f, 3.3f)]
        public void Float_Attribute_Correctly_Implements_Scaling(float df, float scale)
        {
            FloatAttribute val = new FloatAttribute(df);
            float sc = val.AddPercentModifier(scale).Value;

            Assert.IsTrue(sc == df * scale);
        }

        [Test]
        public void Float_Attribute_Correctly_Implements_Clamping()
        {
            var at = new FloatAttribute(5, 10);
            at.Decrease(1);

            Assert.IsTrue(at.Value == 5);
            at.Increase(99);
            Assert.IsTrue(at.Value == 10);
        }
        #endregion
    }
}