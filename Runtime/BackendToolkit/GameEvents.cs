using System;
using System.Collections.Generic;

namespace INUlib.BackendToolkit
{
    public interface IGameEvent { }

    public class GameEventsManager
    {
        #region Fields
        private Dictionary<Type, List<object>> m_subscribers;
        #endregion Fields
        
        #region Constructor
        public GameEventsManager()
        {
            m_subscribers = new Dictionary<Type, List<object>>();
        }
        #endregion Constructor
        
        
        #region Methods
        public void Subscribe<T1>(Action<T1> callback) where T1 : class, IGameEvent
        {
            var key = typeof(T1);
            if(!m_subscribers.ContainsKey(key))
                m_subscribers.Add(key, new List<object>());			
			
            m_subscribers[key].Add(callback);
        }
		
        public void Unsubscribe<T1>(Action<T1> callback) where T1 : class, IGameEvent
        {
            var key = typeof(T1);
            if(!m_subscribers.ContainsKey(key))
                return;
			
            m_subscribers[key].Remove(callback);
        }
        
        public void Invoke<T1>(T1 evt) where T1 : class, IGameEvent
        {
            List<object> subs = GetSubscribers<T1>();
            if(subs != null && subs.Count > 0)
                foreach(object sub in subs)
                    ((Action<T1>)sub).Invoke(evt);
        }
        #endregion Methods
        
        
        #region Helper Methods
        private List<object> GetSubscribers<T1>() where T1 : class, IGameEvent
        {
            if(m_subscribers.ContainsKey(typeof(T1)))
                return m_subscribers[typeof(T1)];
			   
            return null;
        }
        #endregion Helper Methods
    }
}