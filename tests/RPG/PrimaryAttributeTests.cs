using System;
using INUlib.RPG.RPGAttributes;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Runtime.RPG.Attributes
{
    public class PrimaryAttributeTests
    {
        #region Setup
        private float _dfValue = 10;
        private float _minValue = 1;
        private float _maxValue = 100;
        private bool _changed;
        private PrimaryAttribute _mockAttr;

        [SetUp]
        public void Setup()
        {
            _changed = false;
            _mockAttr = Substitute.ForPartsOf<PrimaryAttribute>(AttributeType.Float, _dfValue, _minValue, _maxValue);
            _mockAttr.onAttributeChanged += () => _changed = true;
            _mockAttr.When(x => x.CalculateMods()).Do(x => {
                float total = 0;
                foreach(var pctMod in _mockAttr.PercentMods)
                    total += pctMod.ValueAsFloat();
                foreach(var flatMod in _mockAttr.FlatMods)
                    total += flatMod.ValueAsFloat();
            
                if(total < 0)
                    total = 0;

                _mockAttr.ModsValue.Returns(total);
            });
        }
        #endregion


        #region Tests
        [Test]
        public void PrimaryAttribute_Raises_Changed_On_Increasing_Float()
        {
            _mockAttr.Increase(5f);
            Assert.IsTrue(_changed);
        }

        [Test]
        public void PrimaryAttribute_Raises_Changed_On_Increasing_Int()
        {
            _mockAttr.Increase(5);
            Assert.IsTrue(_changed);
        }

        [Test]
        public void PrimaryAttribute_Raises_Changed_On_Decreasing_Float()
        {
            _mockAttr.Decrease(5f);
            Assert.IsTrue(_changed);
        }

        [Test]
        public void PrimaryAttribute_Raises_Changed_On_Decreasing_Int()
        {
            _mockAttr.Decrease(5);
            Assert.IsTrue(_changed);
        }

        [Test]
        public void PrimaryAttribute_Clamps_To_Minimum_Value_When_Decreasing()
        {
            _mockAttr.Increase(10);
            _mockAttr.Decrease(100f);

            Assert.AreEqual(_minValue, _mockAttr.CurrentValue);
        }

        [Test]
        public void PrimaryAttribute_Clamps_To_Max_Value_When_Increasing()
        {
            _mockAttr.Increase(300f);
            Assert.AreEqual(_maxValue, _mockAttr.CurrentValue);
        }

        [Test]
        public void PrimaryAttribute_Wont_Clamp_When_Increasing_Without_Max_Val()
        {
            _mockAttr = Substitute.ForPartsOf<PrimaryAttribute>(AttributeType.Float, _dfValue, _minValue);
            _mockAttr.Increase(300f);
            Assert.AreEqual(300f + _dfValue, _mockAttr.CurrentValue);
        }

        [Test]
        [TestCase(105.5f, 105)]
        [TestCase(17.321f, 17)]
        [TestCase(5f, 5)]
        [TestCase(9.8786f, 9)]
        public void PrimaryAttribute_Correctly_Increases_With_Float_When_Type_Is_Integer(float inc, int expected)
        {
            _mockAttr = Substitute.ForPartsOf<PrimaryAttribute>(AttributeType.Integer, _dfValue, _minValue);
            _mockAttr.Increase(inc);

            Assert.AreEqual((float)expected + _dfValue, _mockAttr.CurrentValue);
        }

        [Test]
        [TestCase(110, 105.5f, 5)]
        [TestCase(24, 17.321f, 7)]
        [TestCase(99, 5f, 94)]
        [TestCase(10, 9.8786f, 1)]
        [TestCase(10, 20.3f, 1)]
        public void PrimaryAttribute_Correctly_Decreases_With_Float_When_Type_Is_Integer(int startingVal, float dec, int expected)
        {
            _mockAttr = Substitute.ForPartsOf<PrimaryAttribute>(AttributeType.Integer, startingVal, _minValue);
            _mockAttr.Decrease(dec);
            Assert.AreEqual((float)expected, _mockAttr.CurrentValue);
        }

        [Test]
        [TestCase(105.5f)]
        [TestCase(17.321f)]
        [TestCase(5f)]
        [TestCase(9.8786f)]
        public void PrimaryAttribute_Correctly_Increases_With_Float_When_Type_Is_Float(float inc)
        {
            _mockAttr = Substitute.ForPartsOf<PrimaryAttribute>(AttributeType.Float, _dfValue, _minValue);
            _mockAttr.Increase(inc);

            Assert.AreEqual(inc + _dfValue, _mockAttr.CurrentValue);
        }

        [Test]
        [TestCase(110f, 105.5f, 4.5f)]
        [TestCase(24f, 17.3f, 6.7f)]
        [TestCase(99f, 5f, 94f)]
        [TestCase(11f, 9.43f, 1.57f)]
        [TestCase(11f, 12f, 1f)]
        public void PrimaryAttribute_Correctly_Decreases_With_Float_When_Type_Is_Float(float startingVal, float dec, float expected)
        {
            _mockAttr = Substitute.ForPartsOf<PrimaryAttribute>(AttributeType.Float, startingVal, _minValue);
            _mockAttr.Decrease(dec);

            // Need to check range instead of value since there's floating point imprecision
            Assert.IsTrue(InRange(_mockAttr.CurrentValue, expected));
        }

        [Test]
        public void PrimaryIntAttribute_Correctly_Calculates_Mods()
        {
            PrimaryIntAttribute attr = new PrimaryIntAttribute(5, 1);
            attr.AddFlatModifier(10.5f);
            attr.AddFlatModifier(12.5f);
            attr.AddPercentModifier(2);

            Assert.AreEqual(32, attr.ModsValue);
        }

        [Test]
        public void PrimaryFloatAttribute_Correctly_Calculates_Mods()
        {
            PrimaryFloatAttribute attr = new PrimaryFloatAttribute(5f, 1f);
            attr.AddFlatModifier(10.5f);
            attr.AddFlatModifier(12.2f);
            attr.AddPercentModifier(2.5f);

            Assert.AreEqual(35.2f, attr.ModsValue);
        }

        [Test]
        [TestCase(1f)]
        [TestCase(2f)]
        [TestCase(2.5f)]
        [TestCase(5f)]
        public void PrimaryAttribute_Refreshes_Mods_Value_With_Float_Increase(float pctIncrease)
        {
            IAttributeMod pctMod = _mockAttr.AddPercentModifier(pctIncrease);
            _mockAttr.Increase(_dfValue);

            Assert.AreEqual(2*_dfValue*pctIncrease, pctMod.ValueAsFloat());
        }

        [Test]
        [TestCase(1f)]
        [TestCase(2f)]
        [TestCase(2.5f)]
        [TestCase(5f)]
        public void PrimaryAttribute_Refreshes_Mods_Value_With_Integer_Increase(float pctIncrease)
        {
            IAttributeMod pctMod = _mockAttr.AddPercentModifier(pctIncrease);
            _mockAttr.Increase((int)_dfValue);

            Assert.AreEqual(2*_dfValue*pctIncrease, pctMod.ValueAsFloat());
        }

        [Test]
        [TestCase(1f)]
        [TestCase(2f)]
        [TestCase(2.5f)]
        [TestCase(5f)]
        public void PrimaryAttribute_Refreshes_Mods_Value_With_Float_Decrease(float pctIncrease)
        {
            IAttributeMod pctMod = _mockAttr.AddPercentModifier(pctIncrease);
            _mockAttr.Increase(_dfValue);
            _mockAttr.Decrease(_dfValue);

            Assert.AreEqual(_dfValue*pctIncrease, pctMod.ValueAsFloat());
        }

        [Test]
        [TestCase(1f)]
        [TestCase(2f)]
        [TestCase(2.5f)]
        [TestCase(5f)]
        public void PrimaryAttribute_Refreshes_Mods_Value_With_Int_Decrease(float pctIncrease)
        {
            IAttributeMod pctMod = _mockAttr.AddPercentModifier(pctIncrease);
            _mockAttr.Increase((int)_dfValue);
            _mockAttr.Decrease((int)_dfValue);

            Assert.AreEqual(_dfValue*pctIncrease, pctMod.ValueAsFloat());
        }
        #endregion


        #region Helper Methods
        private bool InRange(float value, float expected) => MathF.Abs(expected - value) < 0.001f;
        #endregion
    }
}