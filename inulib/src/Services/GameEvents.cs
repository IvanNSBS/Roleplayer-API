using System;
using System.Linq;
using System.Collections.Generic;

namespace INUlib.Services
{
    public interface IEventArgs { }
    
    public interface IEventHandler
    {
        int EventId { get; }

        public Type GetEventArgsType();
        bool UnboxAndInvoke(params object[] args);
        bool Invoke(IEventArgs args);
        void AddSubscriber(object clbk);
        void RemoveSubscriber(object clbk);
    }

    public abstract class BaseEventHandler<T> : IEventHandler where T : class, IEventArgs
    {
        protected List<Action<T>> _clbks;
        public abstract int EventId { get; }
        public BaseEventHandler() => _clbks = new List<Action<T>>();

        /// <summary>
        /// Unboxes the param args, constructing the appropriate IEventArgs
        /// and invoking the event
        /// </summary>
        /// <param name="args">The params to be unboxed on a IEventArgs type</param>
        /// <returns>
        /// True if the args was valid and the appropriate IEventArgs was instantiated.
        /// False otherwise
        /// </returns>
        public bool UnboxAndInvoke(params object[] args)
        {
            T @params = Unbox(args);
            return Invoke(@params);
        }

        /// <summary>
        /// Invokes the event given the IEventArgs
        /// </summary>
        /// <param name="args">The event arguments</param>
        /// <returns>True if event args was not null. False otherwise</returns>
        public bool Invoke(IEventArgs args)
        {
            if (args == null)
                return false;
            
            foreach(var clbk in _clbks)
                clbk?.Invoke((T)args);

            return true;
        }

        public Type GetEventArgsType() => typeof(T);
        protected abstract T Unbox(params object[] args);
        public void AddSubscriber(object clbk) => _clbks.Add((Action<T>)clbk);    
        public void RemoveSubscriber(object clbk) => _clbks.Remove((Action<T>)clbk);
    }

    public class GameEventsManager
    {
        #region Fields
        // IGameEventUnboxers are automatically filled through reflection
        private Dictionary<Type, int> _handlerPointers;
        private Dictionary<int, IEventHandler> _handlers;
        #endregion
        
        
        #region Methods
        public GameEventsManager(bool autoInstantiateHandlers)
        {
            _handlerPointers = new Dictionary<Type, int>();
            _handlers = new Dictionary<int, IEventHandler>();
            
            if(autoInstantiateHandlers)
                GenerateAllEventHandlersThroughReflection();
        }
        
        public void Subscribe<T>(Action<T> callback) where T : class, IEventArgs
        {
            Type t = typeof(T);
            int eventId = _handlerPointers[t];
            _handlers[eventId].AddSubscriber(callback);
        }
        
        public void Unsubscribe<T>(Action<T> callback) where T : class, IEventArgs
        {
            Type t = typeof(T);
            int eventId = _handlerPointers[t];
            _handlers[eventId].RemoveSubscriber(callback);
        }
        
        /// <summary>
        /// Invokes the event, given the id and object params
        /// </summary>
        /// <param name="eventId">The id of the event handler</param>
        /// <param name="args">The event params</param>
        /// <returns>
        /// True if there was an event handler for the event id and the args were properly unboxed by it.
        /// False if there is no event handler for the event id or the args were invalid
        /// </returns>
        public bool Invoke(int eventId, params object[] args)
        {
            if (!_handlers.ContainsKey(eventId))
                return false;
            
            var unboxer = _handlers[eventId];
            return unboxer.UnboxAndInvoke(args);
        }

        /// <summary>
        /// Invokes the event, EventArgs
        /// </summary>
        /// <param name="event">The event arguments class</param>
        /// <returns>
        /// True if there was an event handler for the event and the event args was not null.
        /// False otherwise
        /// </returns>
        public bool Invoke<T>(T @event) where T : class, IEventArgs
        {
            Type t = typeof(T);
            if (!_handlerPointers.ContainsKey(t))
                return false;
            
            int eventId = _handlerPointers[t];
            var unboxer = _handlers[eventId];
            return unboxer.Invoke(@event);
        }

        public void AddEventHandler(IEventHandler handler)
        {
            _handlerPointers.Add(handler.GetEventArgsType(), handler.EventId);
            _handlers.Add(handler.EventId, handler);
        }

        public bool RemoveEventHandler(IEventHandler handler)
        {
            _handlerPointers.Remove(handler.GetType());
            return _handlers.Remove(handler.EventId);
        }

        public bool RemoveEventHandler(int eventId)
        {
            var key = _handlerPointers.FirstOrDefault(x => x.Value == eventId).Key;
            if (key == null)
                return false;

            _handlerPointers.Remove(key);
            return _handlers.Remove(eventId);
        }

        public bool RemoveEventHandler(IEventArgs args)
        {
            if (!_handlerPointers.ContainsKey(args.GetType()))
                return false;

            _handlers.Remove(_handlerPointers[args.GetType()]);
            return _handlerPointers.Remove(args.GetType());
        }

        public bool HasEventHandler(IEventHandler handler) => _handlers.ContainsKey(handler.EventId);
        public bool HasEventHandlerFor(int eventId) => _handlers.ContainsKey(eventId);
        public bool HasEventHandlerFor(IEventArgs args) => _handlerPointers.ContainsKey(args.GetType());
        #endregion
        
        
        #region Helper Methods
        private void GenerateAllEventHandlersThroughReflection()
        {
            var type = typeof(IEventHandler);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p));

            foreach(Type handler in types)
            {
                IEventHandler instance = (IEventHandler)Activator.CreateInstance(handler);
                AddEventHandler(instance);
            }
        }
        #endregion
    }
}