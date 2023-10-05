using System;
using INUlib.Services;
using NUnit.Framework;

namespace Tests.Runtime.Services.Events
{
    public class EventsTests
    {
        #region Example Classes
        public class TestEventArgs : IEventArgs
        {	
            public int value;
            public TestEventArgs(int value)
            {
                this.value = value;
            }
        }

        public class TestEventHandler : BaseEventHandler<TestEventArgs>
        {
            public override int EventId => 0;
            
            protected override TestEventArgs Unbox(params object[] args)
            {
                if (args.Length < 1 || !(args[0] is int))
                    return null;

                return new TestEventArgs((int)args[0]);
            }
        }
        #endregion Example Classes
        
        #region Setup
        private GameEventsManager _manager;
        private TestEventHandler _testHandler;
        
        [SetUp]
        public void Setup()
        {
            _manager = new GameEventsManager(false);
            _manager.AddEventHandler(_testHandler = new TestEventHandler());
        }
        #endregion
        
        #region Tests

        [Test]
        public void EventHandler_Can_Be_Accessed_Through_EventId()
        {
            int eventId = _testHandler.EventId;
            Assert.IsTrue(
                _manager.HasEventHandlerFor(eventId), 
                $"There should've been an event handler for event id: {eventId}"
            );
        }

        [Test]
        public void EventHandler_Can_Be_Accessed_Through_EventArgs()
        {
            TestEventArgs args = new TestEventArgs(0);
            string argsName = args.GetType().Name;
            Assert.IsTrue(
                _manager.HasEventHandlerFor(args), 
                $"There should've been an event handler for EventArgs of type: {argsName}"
            );
        }

        [Test]
        public void EventHandler_Can_Be_Removed_Through_HandlerReference()
        {
            _manager.RemoveEventHandler(_testHandler);

            Assert.IsFalse(
                _manager.HasEventHandler(_testHandler), 
                $"There should NOT have been an event handler for EventId: {_testHandler.GetType().Name}"
            );
        }
        
        [Test]
        public void EventHandler_Can_Be_Removed_Through_EventId()
        {
            int eventId = _testHandler.EventId;
            _manager.RemoveEventHandler(eventId);

            Assert.IsFalse(
                _manager.HasEventHandlerFor(eventId), 
                $"There should NOT have been an event handler for EventId: {eventId}"
            );
        }
        
        [Test]
        public void EventHandler_Can_Be_Removed_Through_EventArgs()
        {
            TestEventArgs args = new TestEventArgs(0);
            string argsName = args.GetType().Name;

            _manager.RemoveEventHandler(args);
            
            Assert.IsFalse(
                _manager.HasEventHandlerFor(args), 
                $"There should NOT have been an event handler for EventArgs of type: {argsName}"
            );
        }
        
        [Test]
        public void Subscriber_Is_Added_And_Fired()
        {
            int  actual = 0;
            
            Action<TestEventArgs> listener = evt =>  actual = evt.value;
            _manager.Subscribe(listener);
            _manager.Invoke(new TestEventArgs(10));
            
            Assert.AreEqual(10, actual);
        }
        
        [Test]
        public void Multiple_Subscribers_Are_Added_And_Fired()
        {
            int actual = 0;
            
            Action<TestEventArgs> l1 = evt => actual += evt.value;
            Action<TestEventArgs> l2 = evt => actual += evt.value;
            _manager.Subscribe(l1);
            _manager.Subscribe(l2);
            _manager.Invoke(new TestEventArgs(10));
            
            Assert.AreEqual(20, actual);
        }
        
        [Test]
        public void Subscriber_Is_Removed()
        {
            int actual = 0;
            
            Action<TestEventArgs> listener = evt => actual = evt.value;
            _manager.Subscribe(listener);
            _manager.Unsubscribe(listener);
            _manager.Invoke(new TestEventArgs(10));
            
            Assert.AreEqual(0, actual);
        }
        
        [Test]
        public void Multiple_Subscribers_Are_Removed()
        {
            int actual = 0;
            
            Action<TestEventArgs> l1 = evt => actual += evt.value;
            Action<TestEventArgs> l2 = evt => actual += evt.value;
            _manager.Subscribe(l1);
            _manager.Subscribe(l2);
            _manager.Unsubscribe(l1);
            _manager.Unsubscribe(l2);
            _manager.Invoke(new TestEventArgs(10));
            
            Assert.AreEqual(0, actual);
        }

        [Test]
        public void Event_Is_Fired_From_Id_And_Object_Params()
        {
            int testVal = 0;
            Action<TestEventArgs> listener = evt => testVal = evt.value;

            _manager.Subscribe(listener);
            _manager.Invoke(0, 10);
        }

        [Test]
        public void Event_Wont_Be_Fired_If_There_Is_No_Event_Handler_For_It()
        {
            _manager = new GameEventsManager(false);
            bool invoked = _manager.Invoke(0, 10);
            
            Assert.IsFalse(invoked);
        }

        [Test]
        public void Event_Wont_Fire_If_The_Wrong_Params_Were_Used()
        {
            bool invoked = _manager.Invoke(0, "10");
            Assert.IsFalse(invoked);
        }

        [Test]
        public void Event_Wont_Fire_If_EventArgs_Was_Null()
        {
            bool invoked = _manager.Invoke<TestEventArgs>(null);
            Assert.IsFalse(invoked);
        }
        #endregion Tests
    }
}