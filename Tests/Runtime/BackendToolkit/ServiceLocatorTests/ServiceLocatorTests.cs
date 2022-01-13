using NUnit.Framework;
using INUlib.BackendToolkit;

namespace Tests.Runtime.BackendToolkit.ServiceLocatorTests
{
    [TestFixture]
    public class ServiceLocatorTests
    {
        #region Setup
        private interface IMockInterface { int MockInt { get; } }
        private class MockClass : IMockInterface { public int MockInt => 5; }

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
            ServiceLocator.InitSingleton();
            Assert.NotNull(ServiceLocator.Instance);
        }

        [Test]
        public void Service_Is_Registered()
        {
            bool added = _locator.RegisterService<MockClass>(new MockClass());
            Assert.IsTrue(added);
        }

        [Test]
        public void Can_Get_Service()
        {
            bool added = _locator.RegisterService<MockClass>(new MockClass());
            var mock = _locator.GetService<MockClass>();

            Assert.NotNull(mock);
        }

        [Test]
        public void Service_Is_Removed()
        {
            bool added = _locator.RegisterService<MockClass>(new MockClass());
            bool removed = _locator.RemoveService<MockClass>();

            Assert.IsTrue(added && removed);
        }

        [Test]
        public void Can_Register_Service_By_Interface_With_Implementation_Class()
        {
            bool added = _locator.RegisterService<IMockInterface>(new MockClass());
            Assert.IsTrue(added);
        }

        [Test]
        public void Can_Unregister_Service_By_Interface_With_Implementation_Class()
        {
            bool added = _locator.RegisterService<IMockInterface>(new MockClass());
            bool removed = _locator.RemoveService<IMockInterface>();
            Assert.IsTrue(added && removed);
        }

        [Test]
        public void Can_Retrieve_Service_By_Interface_Using_Implementation_Class()
        {
            _locator.RegisterService<IMockInterface>(new MockClass());
            var mock = _locator.GetService<IMockInterface>();

            Assert.NotNull(mock);
            Assert.IsTrue(mock.MockInt == 5);
        }
        #endregion
    }
}