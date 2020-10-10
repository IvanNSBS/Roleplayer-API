using System.Collections.Generic;

namespace RPGCore.FileManagement.SavingFramework
{
    public class SaveManager
    {
        #region Fields
        private List<Saveable> subscribers;
        #endregion Fields
        
        #region Singleton
        private static SaveManager _instance;
        public static SaveManager Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new SaveManager();

                return _instance;
            }
        }
        #endregion Singleton
        
        
        #region Constructors
        private SaveManager()
        {
            subscribers = new List<Saveable>();            
        }
        #endregion Constructors
        

        #region Savegame Events
        public void Save()
        {
                        
        }

        public void Load()
        {
            
        }
        #endregion Savegame Events
        
        
        #region Methods
        public void AddSubscriber(Saveable saveable)
        {
            subscribers.Add(saveable);
        }

        public bool RemoveSubscriber(Saveable saveable)
        {
            return subscribers.Remove(saveable);
        }
        #endregion Methods
    }
}