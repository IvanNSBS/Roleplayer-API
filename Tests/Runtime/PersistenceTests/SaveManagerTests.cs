using System;
using Newtonsoft.Json;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using Essentials.Persistence;
using Essentials.Persistence.Interfaces;

namespace Tests.Runtime.PersistenceTests
{
    [TestFixture]
    public class SaveManagerTests
    {
        #region Dummy Classes
        class DataStoreMock : DataStore
        {
            private DataStoreElementMock m_mock;
            
            public DataStoreMock(SaveManager saveManager) : base(saveManager)
            {
            }

            public override JObject SerializeStoredData() => m_mock.ToJson();

            public override bool DeserializeStoredObjectsFromCache()
            {
                m_mock.FromJson(dataStoreCache);
                return true;
            }

            public override void ClearSaveStore()
            {
                dataStoreCache = null;
                m_mock = null;
            }

            public override void RemoveStoredObjects() => m_mock = null;

            public void RegisterMockData(DataStoreElementMock el) => m_mock = el;
        }

        class DataStoreElementMock : IDataStoreElement
        {
            public DataStoreElementMock()
            {
                data = new SerializedClass();
            }
            
            #region Serialized Data
            [Serializable]
            public class SerializedClass
            {
                [JsonProperty] public string m_data;
            }
            
            [JsonProperty] public SerializedClass data;
            #endregion Serialized Data
            
            public string ObjectId { get; }
            public JObject ToJson()
            {
                var jsonString = JsonConvert.SerializeObject(data);
                return JObject.Parse(jsonString);
            }

            public void FromJson(JObject objectJson)
            {
                data = JsonConvert.DeserializeObject<SerializedClass>(objectJson.ToString());
            }

            public void RegisterToDataStore()
            {
                SaveManager.Instance.GetDataStoreFor<DataStoreMock>().RegisterMockData(this);
            }

            public void RemoveFromDataStore()
            {
                SaveManager.Instance.GetDataStoreFor<DataStoreMock>().RemoveStoredObjects();
            }
        }
        
        #endregion Dummy Classes

        #region Helper Methods
        [TearDown][SetUp]
        public void ResetSingleton()
        {
            DataStoreRegistry.ClearRegistry();
            SaveManager.ResetDataStores();
        }
        #endregion Helper Methods
        
        #region Tests
        [Test]
        public void DataStoreGetsProperlyRegistered()
        {
            DataStoreRegistry.AddStoreToRegistry<DataStoreMock>();
            Assert.IsNotNull(SaveManager.Instance.GetDataStoreFor<DataStoreMock>());
        }

        [Test]
        public void SaveFileIsCreated()
        {
            SaveManager.Instance.Save();
            Assert.IsTrue(SaveManager.Instance.DeleteSaveFileFromDisk());
        }
        
        [Test]
        public void SaveFileContentIsProperlySet()
        {
            DataStoreRegistry.AddStoreToRegistry<DataStoreMock>();
            var store = SaveManager.Instance.GetDataStoreFor<DataStoreMock>();

            var mock = new DataStoreElementMock();
            mock.data.m_data = "Mockup of Store Data";
            
            store.RegisterMockData(mock);
            var expectedJson = store.SerializeStoredData().ToString();
            SaveManager.Instance.Save();
            
            
            mock.data.m_data = "Overriding Value";
            var changedJson = store.SerializeStoredData().ToString();
            SaveManager.Instance.LoadStoresCacheFromSaveFile(true);
            var loadedJson = store.SerializeStoredData().ToString();
            
            Assert.IsTrue(loadedJson == expectedJson && loadedJson != changedJson);
            
            SaveManager.Instance.DeleteSaveFileFromDisk();
        }
        #endregion Tests
    }
}