using INUlib.RPG.CharacterSheet;
using NUnit.Framework;

namespace Tests.Runtime.RPG.Attributes
{
    public class RPGAttributesMathTests
    {
        [Test]
        [TestCase(1, 10)]
        [TestCase(3, 123)]
        [TestCase(4, 0)]
        [TestCase(54, 23)]
        public void Int_Number_Sum(int a, int b)
        {
            int val = new Int(a) + new Int(b);
            int v1 = new Int(a) + b;
            int v2 = a + new Int(b);
            Assert.AreEqual(a + b, val);
            Assert.AreEqual(a + b, v1);
            Assert.AreEqual(a + b, v2);
        }

        [Test]
        [TestCase(2, 2.0f)]
        [TestCase(1, 4.0f)]
        [TestCase(0, 9.0f)]
        [TestCase(32, 3.5f)]
        public void Int_Number_Sum_Float(int a, float b)
        {
            int val = new Int(a) + new Float(b);
            int v1 = new Int(a) + b;
            int v2 = b + new Int(a);
            Assert.AreEqual(a + (int)b, val);
            Assert.AreEqual(a + (int)b, v1);
            Assert.AreEqual(a + (int)b, v2);
        }

        [Test]
        [TestCase(1, 10)]
        [TestCase(3, 123)]
        [TestCase(4, 0)]
        [TestCase(54, 23)]
        public void Float_Number_Sum(float a, float b)
        {
            float val = new Float(a) + new Float(b);
            float v1 = new Float(b) + a;
            float v2 = a + new Float(b);
            Assert.AreEqual(a + b, val);
            Assert.AreEqual(a + b, v1);
            Assert.AreEqual(a + b, v2);
        }

        [Test]
        [TestCase(2, 2.0f)]
        [TestCase(1, 4.0f)]
        [TestCase(0, 9.0f)]
        [TestCase(32, 3.5f)]
        public void Float_Number_Sum_Int(int a, float b)
        {
            float val = new Float(b) + new Int(a);
            float v1 = new Float(b) + a;
            float v2 = a + new Float(b);
            Assert.AreEqual(a + b, val);
            Assert.AreEqual(a + b, v1);
            Assert.AreEqual(a + b, v2);
        }

        [Test]
        [TestCase(5, 3)]
        [TestCase(2, 1)]
        [TestCase(1, 32)]
        [TestCase(4, 99)]
        public void Int_Number_Subtraction(int a, int b)
        {
            int val = new Int(a) - new Int(b);
            int v1 = new Int(a) - b;
            int v2 = a - new Int(b);
            Assert.AreEqual(a - b, val);
            Assert.AreEqual(a - b, v1);
            Assert.AreEqual(a - b, v2);
        }

        [Test]
        [TestCase(5, 3f)]
        [TestCase(2, 1f)]
        [TestCase(1, 32f)]
        [TestCase(4, 99f)]
        public void Int_Number_Subtraction_Float(int a, float b)
        {
            int val = new Int(a) - new Float(b);
            int v1 = new Int(a) - b;
            Assert.AreEqual(a - (int)b, val);
            Assert.AreEqual(a - (int)b, v1);
        }

        [Test]
        [TestCase(5f, 3.5f)]
        [TestCase(2f, 1.2f)]
        [TestCase(1.23f, 32.9f)]
        [TestCase(4.15f, 32f)]
        public void Float_Number_Subtraction(float a, float b)
        {
            float val = new Float(a) - new Float(b);
            float v1 = new Float(a) - b;
            float v2 = a - new Float(b);
            Assert.AreEqual(a - b, val);
            Assert.AreEqual(a - b, v1);
            Assert.AreEqual(a - b, v2);
        }

        [Test]
        [TestCase(5f, 3)]
        [TestCase(2.3f, 1)]
        [TestCase(1.98f, 32)]
        [TestCase(4.223f, 99)]
        public void Float_Number_Subtraction_Int(float a, int b)
        {
            float val = new Float(a) - new Int(b);
            float v1 = new Float(a) - b;
            float v2 = a - new Float(b);
            Assert.AreEqual(a - b, val);
            Assert.AreEqual(a - b, v1);
            Assert.AreEqual(a - b, v2);
        }

        [Test]
        [TestCase(5, 2)]
        [TestCase(2, 1)]
        [TestCase(32, 32)]
        [TestCase(4, 2)]
        [TestCase(8, 3)]
        [TestCase(8, 2)]
        public void Int_Number_Division(int a, int b)
        {
            int val = new Int(a)/new Int(b);
            int v1 = new Int(a)/b;
            int v2 = a/new Int(b);
            Assert.AreEqual(a/b, val);
            Assert.AreEqual(a/b, v1);
            Assert.AreEqual(a/b, v2);
        }

        [Test]
        [TestCase(5, 2.0f)]
        [TestCase(2, 1.5f)]
        [TestCase(32, 1.8f)]
        [TestCase(4, 2.2f)]
        [TestCase(8, 3.5f)]
        [TestCase(8, 2.0f)]
        public void Int_Number_Division_Float(int a, float b)
        {
            int val = new Int(a)/new Float(b);
            int v1 = new Int(a) / b;
            int v2 = a / new Float(b);
            Assert.AreEqual((int)(a/b), val);
            Assert.AreEqual((int)(a/b), v1);
            Assert.AreEqual((int)(a/b), v2);
        }

        [Test]
        [TestCase(5, 2)]
        [TestCase(2, 1)]
        [TestCase(32, 32)]
        [TestCase(4, 2)]
        [TestCase(8, 3)]
        [TestCase(8, 2)]
        public void Float_Number_Division(float a, float b)
        {
            float val = new Float(a)/new Float(b);
            float v1 = new Float(a)/b;
            float v2 = a/new Float(b);
            Assert.AreEqual(a/b, val);
            Assert.AreEqual(a/b, v1);
            Assert.AreEqual(a/b, v2);
        }

        [Test]
        [TestCase(5.5f, 2)]
        [TestCase(2.2f, 1)]
        [TestCase(32.5f, 32)]
        [TestCase(4, 2)]
        [TestCase(8, 3)]
        [TestCase(8, 2)]
        public void Float_Number_Division_int(float a, int b)
        {
            float val = new Float(a)/new Int(b);
            float v1 = new Float(a)/b;
            float v2 = a/new Float(b);
            Assert.AreEqual(a/b, val);
            Assert.AreEqual(a/b, v1);
            Assert.AreEqual(a/b, v2);
        }

        [Test]
        [TestCase(5, 2)]
        [TestCase(2, 1)]
        [TestCase(32, 32)]
        [TestCase(4, 2)]
        [TestCase(8, 3)]
        [TestCase(8, 2)]
        public void Int_Number_Multiplication(int a, int b)
        {
            int val = new Int(a)*new Int(b);
            int v1 = new Int(a)*b;
            int v2 = a*new Int(b);
            Assert.AreEqual(a*b, val);
            Assert.AreEqual(a*b, v1);
            Assert.AreEqual(a*b, v2);
        }

        [Test]
        [TestCase(5, 2.0f)]
        [TestCase(2, 1.5f)]
        [TestCase(32, 32)]
        [TestCase(4, 2.0f)]
        [TestCase(8, 3.5f)]
        [TestCase(4, 0.5f)]
        [TestCase(16, 0.25f)]
        public void Int_Number_Multiplication_Float(int a, float b)
        {
            int val = new Int(a)*new Float(b);
            int v1 = new Int(a)*b;
            int v2 = a*new Float(b);
            Assert.AreEqual((int)(a*b), val);
            Assert.AreEqual((int)(a*b), v1);
            Assert.AreEqual((int)(a*b), v2);
        }

        [Test]
        [TestCase(5, 2)]
        [TestCase(2, 1)]
        [TestCase(32, 32)]
        [TestCase(4, 2)]
        [TestCase(8, 3)]
        [TestCase(8, 2)]
        public void Float_Number_Multiplication(float a, float b)
        {
            float val = new Float(a)*new Float(b);
            float v1 = new Float(a) * b;
            float v2 = a * new Float(b);
            Assert.AreEqual(a*b, val);
            Assert.AreEqual(a*b, v1);
            Assert.AreEqual(a*b, v2);
        }

        [Test]
        [TestCase(5.5f, 2)]
        [TestCase(2.2f, 1)]
        [TestCase(32.5f, 32)]
        [TestCase(4, 2)]
        [TestCase(8, 3)]
        [TestCase(8, 2)]
        public void Float_Number_Multiplication_Int(float a, int b)
        {
            float val = new Float(a)*new Int(b);
            float v1 = new Float(a)*b;
            // float v2 = a * new Int(b);
            Assert.AreEqual(a*b, val);
            Assert.AreEqual(a*b, v1);
            // Assert.AreEqual(a*b, v2);
        }
    }
}