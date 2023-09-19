using System;
using INUlib.RPG.RPGAttributes;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Runtime.RPG.Attributes
{
    public class RPGAttributeTests
    {
        #region Setup
        private float _dfValue = 10f;
        private float _minValue = 1f;
        private float _maxValue = 100f;
        private bool _changed = false;
        private RPGAttribute _mockAttr;

        private RPGAttribute Setup(AttributeType t) => Substitute.ForPartsOf<RPGAttribute>(t, _dfValue, _maxValue);

        [SetUp]
        public void SetupMockAttribute()
        {
            _changed = false;
            _mockAttr = Substitute.ForPartsOf<RPGAttribute>(AttributeType.Float, _dfValue, _minValue, _maxValue);

            _mockAttr.onAttributeChanged += () => _changed = true;

            _mockAttr.When(x => x.CalculateMods()).Do(x => {
                float total = 0;
                foreach(var pct in _mockAttr.PercentMods)
                    total += pct.ValueAsFloat();
                foreach(var flat in _mockAttr.FlatMods)
                    total += flat.ValueAsFloat();
                
                _mockAttr.ModsValue.Returns(total);
            });
        }
        #endregion


        #region Tests
        [Test]
        [TestCase(AttributeType.Integer)]
        [TestCase(AttributeType.Float)]
        public void RPG_Attribute_Empty_Constructor_Correctly_Initializes_Attribute(AttributeType type)
        {
            RPGAttribute attr = Substitute.ForPartsOf<RPGAttribute>(type);
            
            Assert.AreEqual(-1, attr.maxValue);
            Assert.AreEqual(0, attr.minValue);
            Assert.AreEqual(0, attr.defaultValue);
            Assert.AreEqual(0, attr.ValueAsFloat());

            Assert.NotNull(attr.FlatMods);
            Assert.NotNull(attr.PercentMods);
        }

        [Test]
        [TestCase(AttributeType.Integer, 2.5f   , 0.0f)]
        [TestCase(AttributeType.Integer, 2f     , 2.4f)]
        [TestCase(AttributeType.Integer, 9.434f , 2.0f)]
        [TestCase(AttributeType.Float, 2.5f     , 9.0f)]
        [TestCase(AttributeType.Float, 32.32f   , 3.0f)]
        [TestCase(AttributeType.Float, 54.98f   , 20.0f)]
        public void RPG_Attribute_Default_Value_Constructor_Correctly_Initializes_Attribute(
            AttributeType type, float dfVal, float minVal
        )
        {
            RPGAttribute attr = Substitute.ForPartsOf<RPGAttribute>(type, dfVal, minVal);
            
            if(type == AttributeType.Integer)
            {
                int expected = (int)dfVal < (int)minVal ? (int)minVal : (int)dfVal; 
                Assert.AreEqual((int)minVal, attr.minValue);
                Assert.AreEqual(expected, attr.defaultValue);
                Assert.AreEqual(expected, attr.ValueAsFloat());
            }
            else
            {
                float expected = dfVal < minVal ? minVal : dfVal; 
                Assert.AreEqual(minVal, attr.minValue);
                Assert.AreEqual(expected, attr.defaultValue);
                Assert.AreEqual(expected, attr.ValueAsFloat());
            }
            
            Assert.AreEqual(-1, attr.maxValue);

            Assert.NotNull(attr.FlatMods);
            Assert.NotNull(attr.PercentMods);
        }

        [Test]
        [TestCase(AttributeType.Integer, 2.5f,  0f, 5f)]
        [TestCase(AttributeType.Integer, 2f,    3f, 4.322f)]
        [TestCase(AttributeType.Integer, 9.434f,1f, 123.3213f)]
        [TestCase(AttributeType.Float, 2.5f,    1f, 998.32f)]
        [TestCase(AttributeType.Float, 32.32f,  1f, 212f)]
        [TestCase(AttributeType.Float, 54.98f,  1f, 100.55f)]
        public void RPG_Attribute_Full_Constructor_Correctly_Initializes_Attribute(
            AttributeType type, float dfVal, float minVal, float maxVal
        )
        {
            RPGAttribute attr = Substitute.ForPartsOf<RPGAttribute>(type, dfVal, minVal, maxVal);
            
            if(type == AttributeType.Integer)
            {
                int expected = (int)dfVal < (int)minVal ? (int)minVal : (int)dfVal; 
                Assert.AreEqual(expected, attr.defaultValue);
                Assert.AreEqual(expected, attr.ValueAsFloat());
                Assert.AreEqual((int)minVal, attr.minValue);
                Assert.AreEqual((int)maxVal, attr.maxValue);
            }
            else
            {
                float expected = dfVal < minVal ? minVal : dfVal; 
                Assert.AreEqual(expected, attr.defaultValue);
                Assert.AreEqual(expected, attr.ValueAsFloat());
                Assert.AreEqual(minVal, attr.minValue);
                Assert.AreEqual(maxVal, attr.maxValue);
            }

            Assert.NotNull(attr.FlatMods);
            Assert.NotNull(attr.PercentMods);
        }

        [Test]
        [TestCase(1.5f)]
        [TestCase(1f)]
        [TestCase(61.5123f)]
        [TestCase(87.325f)]
        public void Integer_Attribute_Correctly_Adds_Flat_Mod(float flatMod)
        {
            RPGAttribute attr = Setup(AttributeType.Integer);
            IAttributeMod mod = attr.AddFlatModifier(flatMod);

            int modVal = (int)flatMod;
            Assert.AreEqual(modVal, mod.ValueAsInt());
            Assert.AreEqual(modVal, mod.ValueAsFloat());
        }

        
        [Test]
        [TestCase(10, 1.5f, 15)]
        [TestCase(5, 2f, 10)]
        [TestCase(12, 0.5f, 6)]
        [TestCase(3, 1.3f, 3)]
        public void Integer_Attribute_Correctly_Adds_Percent_Mod(int dfValue, float pctMod, int expected)
        {
            RPGAttribute attr = Substitute.ForPartsOf<RPGAttribute>(AttributeType.Integer, dfValue, _minValue);
            IAttributeMod mod = attr.AddPercentModifier(pctMod);

            Assert.AreEqual(expected, mod.ValueAsInt());
            Assert.AreEqual(expected, mod.ValueAsFloat());
        }

        [Test]
        [TestCase(1.5f)]
        [TestCase(1f)]
        [TestCase(61.5123f)]
        [TestCase(87.325f)]
        public void Float_Attribute_Correctly_Adds_Flat_Mod(float flatMod)
        {
            RPGAttribute attr = Setup(AttributeType.Float);
            IAttributeMod mod = attr.AddFlatModifier(flatMod);
            
            float modVal = flatMod;
            Assert.AreEqual((int)modVal, mod.ValueAsInt());
            Assert.AreEqual(modVal, mod.ValueAsFloat());
        }

        
        [Test]
        [TestCase(10, 1.5f)]
        [TestCase(5, 2f)]
        [TestCase(12, 0.5f)]
        [TestCase(3, 1.3f)]
        public void Float_Attribute_Correctly_Adds_Percent_Mod(float dfValue, float pctMod)
        {
            RPGAttribute attr = Substitute.ForPartsOf<RPGAttribute>(AttributeType.Float, dfValue, _minValue);
            IAttributeMod mod = attr.AddPercentModifier(pctMod);
            
            float expected = dfValue * pctMod;
            Assert.AreEqual((int)expected, mod.ValueAsInt());
            Assert.AreEqual(expected, mod.ValueAsFloat());
        }
        #endregion


        #region Tests with Mock Setup
        [Test]
        public void RPGAttribute_Correctly_Removes_Flat_Modifier()
        {
            IAttributeMod mod = _mockAttr.AddFlatModifier(5);
            bool removed = _mockAttr.RemoveFlatModifier(mod);
            
            Assert.IsTrue(removed);
            Assert.IsTrue(_changed);
        }

        [Test]
        public void RPGAttribute_Correctly_Removes_Percent_Modifier()
        {
            IAttributeMod mod = _mockAttr.AddPercentModifier(1);
            bool removed = _mockAttr.RemovePercentModifier(mod);

            Assert.IsTrue(removed);
            Assert.IsTrue(_changed);
        }

        [Test]
        public void RPGAttribute_Does_Nothing_When_Removing_Inexistent_Flat_Mod()
        {
            IAttributeMod mod = Substitute.For<IAttributeMod>();
            bool removed = _mockAttr.RemoveFlatModifier(mod);

            Assert.IsFalse(removed);
            Assert.IsFalse(_changed);
        }

        [Test]
        public void RPGAttribute_Does_Nothing_When_Removing_Inexistent_Percent_Mod()
        {
            IAttributeMod mod = Substitute.For<IAttributeMod>();
            bool removed = _mockAttr.RemovePercentModifier(mod);

            Assert.IsFalse(removed);
            Assert.IsFalse(_changed);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(10)]
        public void RPGAttribute_Correctly_Changes_Calculated_Mods_After_Adding_Flat_Mod(int iterations)
        {
            float expected = 0;
            for(int i = 0; i < iterations; i++)
            {
                _mockAttr.AddFlatModifier(i + 1);
                expected += (i + 1);
            }

            Assert.AreEqual(expected, _mockAttr.ModsValue);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(10)]
        public void RPGAttribute_Correctly_Changes_Calculated_Mods_After_Adding_Percent_Mod(int iterations)
        {
            float pctIncrease = 1f/(float)iterations;
            if(iterations == 0)
                pctIncrease = 0;

            float expected = 0;
            for(int i = 0; i < iterations; i++)
            {
                IAttributeMod mod = _mockAttr.AddPercentModifier(pctIncrease);
                expected += mod.ValueAsFloat();
            }

            Assert.AreEqual(expected, _mockAttr.ModsValue);
        }

        [Test]
        public void RPGAttribute_Correctly_Changes_Calculated_Mods_After_Removing_Flat_Mod()
        {
            IAttributeMod mod = _mockAttr.AddFlatModifier(5);
            float totalMods = _mockAttr.ModsValue;
            bool removed = _mockAttr.RemoveFlatModifier(mod);
            
            Assert.AreEqual(mod.ValueAsFloat(), totalMods);
            Assert.AreEqual(0f, _mockAttr.ModsValue);
        }

        [Test]
        public void RPGAttribute_Correctly_Changes_Calculated_Mods_After_Removing_Percent_Mod()
        {
            IAttributeMod mod = _mockAttr.AddPercentModifier(1);
            float totalMods = _mockAttr.ModsValue;
            bool removed = _mockAttr.RemovePercentModifier(mod);
            
            Assert.AreEqual(_dfValue, totalMods);
            Assert.AreEqual(0f, _mockAttr.ModsValue);
        }
        #endregion
    }
}