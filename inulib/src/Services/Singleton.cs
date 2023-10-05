namespace INUlib.Services
{
    public abstract class Singleton<T> where T : Singleton<T>, new()
    {
        #region Fields
        protected static T _instance;
        #endregion

        #region Properties
        public static T Instance => _instance;
        #endregion


        public static void InitSingleton() => _instance = new T();
        public static void InitSingleton(T instance) => _instance = instance;
        public static void DestroySingleton() => _instance = null;
    }
}