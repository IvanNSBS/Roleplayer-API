using System;
using INUlib.RPG.RPGAttributes;
using NSubstitute;
using NUnit.Framework;

namespace Tests.Runtime.RPG.Attributes
{
    public class DerivedAttributeTests
    {
        #region Setup
        private class MockDerivedAttribute : DerivedAttribute
        {
            public MockDerivedAttribute() : base(AttributeType.Float) { }
            ~MockDerivedAttribute() => Unlink();

            public override float CalculateMods() => 0f;

            public override float UpdateAttribute()
            {
                float currentValue = 0;
                if(Parents == null)
                    return 0;
                    
                foreach(IAttribute parent in Parents)
                    currentValue++;

                return currentValue;
            }

            public void Link(IAttribute parent, params IAttribute[] others) => LinkParents(parent, others);
            public void Unlink() => UnlinkParents();
        }

        private MockDerivedAttribute _mockAttr;

        [SetUp]
        public void Setup()
        {
            _mockAttr = new MockDerivedAttribute();
        }
        #endregion


        #region Tests
        [Test]
        public void DerivedAttribute_Correctly_Links_One_Parent()
        {
            IAttribute example = Substitute.For<IAttribute>();
            _mockAttr.Link(example);

            Assert.AreEqual(1, _mockAttr.CurrentValue);
        }

        [Test]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(9)]
        [TestCase(15)]
        public void DerivedAttribute_Correctly_Links_More_Than_One_Parent(int parents)
        {
            IAttribute example = Substitute.For<IAttribute>();
            IAttribute[] others = new IAttribute[parents - 1];
            for(int i = 0; i < parents - 1; i++)
                others[i] = Substitute.For<IAttribute>();

            _mockAttr.Link(example, others);
            Assert.AreEqual(parents, _mockAttr.CurrentValue);
        }

        [Test]
        public void DerivedAttribute_Correctly_Unlink_One_Parent()
        {
            IAttribute example = Substitute.For<IAttribute>();
            _mockAttr.Link(example);
            _mockAttr.Unlink();

            Assert.AreEqual(0, _mockAttr.CurrentValue);
        }

        [Test]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(9)]
        [TestCase(15)]
        public void DerivedAttribute_Correctly_Unlinks_More_Than_One(int parents)
        {
            IAttribute example = Substitute.For<IAttribute>();
            IAttribute[] others = new IAttribute[parents - 1];
            for(int i = 0; i < parents - 1; i++)
                others[i] = Substitute.For<IAttribute>();

            _mockAttr.Link(example, others);
            _mockAttr.Unlink();
            Assert.AreEqual(0, _mockAttr.CurrentValue);
        }
        #endregion
    }
}