using NUnit.Framework;
using INUlib.Services;

namespace Tests.Runtime.BackendToolkit.ServiceLocatorTests
{
    [TestFixture]
    public class ServiceLocatorTests
    {
        #region Setup
        private interface IMockInterface { int MockInt { get; } }
        private class MockClass : IMockInterface { public int MockInt => 5; }
        private class MockSingleton : Singleton<MockSingleton> { }

        private ServiceLocator _locator;

        [SetUp]
        public void SetupServiceLocator()
        {
            _locator = new ServiceLocator();
        }
        #endregion


        #region Tests
        [Test]
        public void Singleton_CRTP_Creates_The_Instance()
        {
            MockSingleton.InitSingleton();
            Assert.That(MockSingleton.Instance, Is.Not.Null);
            MockSingleton.DestroySingleton();
        }

        [Test]
        public void Service_Is_Registered()
        {
            bool added = _locator.RegisterService<MockClass>(new MockClass());
            Assert.That(added, Is.True);
        }

        [Test]
        public void Can_Get_Service()
        {
            bool added = _locator.RegisterService<MockClass>(new MockClass());
            var mock = _locator.GetService<MockClass>();

            Assert.That(mock, Is.Not.Null);
        }

        [Test]
        public void Service_Is_Removed()
        {
            bool added = _locator.RegisterService<MockClass>(new MockClass());
            bool removed = _locator.RemoveService<MockClass>();

            Assert.That(added && removed, Is.True);
        }

        [Test]
        public void Can_Register_Service_By_Interface_With_Implementation_Class()
        {
            bool added = _locator.RegisterService<IMockInterface>(new MockClass());
            Assert.That(added, Is.True);
        }

        [Test]
        public void Can_Unregister_Service_By_Interface_With_Implementation_Class()
        {
            bool added = _locator.RegisterService<IMockInterface>(new MockClass());
            bool removed = _locator.RemoveService<IMockInterface>();
            Assert.That(added && removed, Is.True);
        }

        [Test]
        public void Can_Retrieve_Service_By_Interface_Using_Implementation_Class()
        {
            _locator.RegisterService<IMockInterface>(new MockClass());
            var mock = _locator.GetService<IMockInterface>();

            Assert.That(mock, Is.Not.Null);
            Assert.That(mock.MockInt, Is.EqualTo(5));
        }
        #endregion
    }
}