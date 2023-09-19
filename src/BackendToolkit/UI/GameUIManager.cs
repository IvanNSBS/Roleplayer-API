using System;
using System.Collections.Generic;
using System.Linq;

namespace INUlib.BackendToolkit.UI
{
    /// <summary>
    /// Manages registration of game views(ui) and game models so they can be attached
    /// through a presenter class, applying the MVP pattern to decouple game UI and logic
    /// </summary>
    public class GameUIManager : Singleton<GameUIManager>
    {
        #region Fields
        private List<IPresenter> _uiPresenters;
        private Dictionary<Type, IGameView> _gameViews;
        private Dictionary<Type, IGameModel> _gameModels;

        public event Action<IGameModel> onModelAdded = delegate { }; 
        public event Action<IGameModel> onModelRemoved = delegate { }; 
        public event Action<IGameView> onViewAdded = delegate { }; 
        public event Action<IGameView> onViewRemoved = delegate { }; 
        #endregion


        #region Constructors
        public GameUIManager() 
        {
            _gameViews = new Dictionary<Type, IGameView>();
            _gameModels = new Dictionary<Type, IGameModel>();
        }

        public GameUIManager(bool useReflectionToInstancePresenters)
        {
            _gameViews = new Dictionary<Type, IGameView>();
            _gameModels = new Dictionary<Type, IGameModel>();
        }
        #endregion


        #region Methods
        public IGameModel GetGameModel(Type modelType)
        {
            if(!_gameModels.ContainsKey(modelType))
                return null;

            return _gameModels[modelType];
        }

        public IGameView GetGameView(Type viewType)
        {
            if(!_gameViews.ContainsKey(viewType))
                return null;

            return _gameViews[viewType];
        }

        public void PushGameUI(IGameView view)
        {
            _gameViews.Add(view.GetType(), view);
            onViewAdded?.Invoke(view);
        }

        public void PopGameUI(IGameView view)
        {
            _gameViews.Remove(view.GetType());
            onViewRemoved?.Invoke(view);
        }

        public void PushGameModel(IGameModel model)
        {
            _gameModels.Add(model.GetType(), model);
            onModelAdded?.Invoke(model);
        }

        public void PopGameModel(IGameModel model)
        {
            _gameModels.Remove(model.GetType());
            onModelRemoved?.Invoke(model);
        }
        #endregion


        #region Helper Methods
        public void CreateAllPresentersThroughReflection()
        {
            _uiPresenters = new List<IPresenter>();
            Type t = typeof(IPresenter);
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(
                s => s.GetTypes()
            ).Where(
                p => t.IsAssignableFrom(p) && !p.IsAbstract
            );
            
            foreach(Type type in types)
                _uiPresenters.Add((IPresenter)Activator.CreateInstance(type));
        }
        #endregion
    }
}