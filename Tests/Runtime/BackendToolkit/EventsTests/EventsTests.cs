using System;
using INUlib.BackendToolkit;
using NUnit.Framework;

namespace Tests.Runtime.BackendToolkit.Events
{
    public class EventsTests
    {
        #region Example Classes
        public class TestEvent : IGameEvent
        {	
            public int value;
            public TestEvent(int value)
            {
                this.value = value;
            }
        }
        #endregion Example Classes
        
        
        #region Tests
        [Test]
        public void SubscriberIsAdded()
        {
            var manager = new GameEventsManager();
            int testVal = 0;
            
            Action<TestEvent> listener = evt => testVal = evt.value;
            manager.Subscribe(listener);
            manager.Invoke(new TestEvent(10));
            
            Assert.IsTrue(testVal == 10);
        }
        
        [Test]
        public void MultipleSubscribersAreAdded()
        {
            var manager = new GameEventsManager();
            int testVal = 0;
            
            Action<TestEvent> l1 = evt => testVal += evt.value;
            Action<TestEvent> l2 = evt => testVal += evt.value;
            manager.Subscribe(l1);
            manager.Subscribe(l2);
            manager.Invoke(new TestEvent(10));
            
            Assert.IsTrue(testVal == 20);
        }
        
        [Test]
        public void SubscriberIsRemoved()
        {
            var manager = new GameEventsManager();
            int testVal = 0;
            
            Action<TestEvent> listener = evt => testVal = evt.value;
            manager.Subscribe(listener);
            manager.Unsubscribe(listener);
            manager.Invoke(new TestEvent(10));
            
            Assert.IsTrue(testVal == 0);
        }
        
        [Test]
        public void MultipleSubscribersAreRemoved()
        {
            var manager = new GameEventsManager();
            int testVal = 0;
            
            Action<TestEvent> l1 = evt => testVal += evt.value;
            Action<TestEvent> l2 = evt => testVal += evt.value;
            manager.Subscribe(l1);
            manager.Subscribe(l2);
            manager.Unsubscribe(l1);
            manager.Unsubscribe(l2);
            manager.Invoke(new TestEvent(10));
            
            Assert.IsTrue(testVal == 0);
        }
        #endregion Tests
    }
}