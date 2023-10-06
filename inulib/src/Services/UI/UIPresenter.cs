namespace INUlib.Services.UI
{
    public interface IPresenter { }

    /// <summary>
    /// Presenter class that defines how a game model and a game view should interact 
    /// between eachother
    /// </summary>
    /// <typeparam name="TView">The view that this presenter works with</typeparam>
    /// <typeparam name="TModel">The model that this presenter works with</typeparam>
    public abstract class UIPresenter<TView, TModel> : IPresenter where TView : class, IGameView where TModel : class, IGameModel
    {
        #region Fields
        protected TModel _model;
        protected TView _view;
        #endregion


        #region Constructor
        public UIPresenter()
        {
            StartObserving();

            _model = GameUIManager.Instance.GetGameModel(typeof(TModel)) as TModel;
            _view = GameUIManager.Instance.GetGameView(typeof(TView)) as TView;

            if(_model != null && _view != null)
                OnAttach();
        }
        #endregion


        #region Abstract Methods
        /// <summary>
        /// Callback when the model and view has been set and the presenter can attach them
        /// </summary>
        protected abstract void OnAttach();

        /// <summary>
        /// Callback when either the model or view has been lost(possiblity deleted) and we should
        /// remove connection within the presenter
        /// </summary>
        protected abstract void OnDetach();
        #endregion


        #region Methods
        /// <summary>
        /// Start observing the GameUI manager to know when the model and view has been
        /// instantiated and check if we can attach or detach
        /// </summary>
        protected void StartObserving()
        {
            GameUIManager.Instance.onModelAdded += ObserveModelAdded;
            GameUIManager.Instance.onViewAdded += ObserveViewAdded;

            GameUIManager.Instance.onModelRemoved += ObserveModelRemoved;
            GameUIManager.Instance.onViewRemoved += ObserveViewRemoved;
        }

        /// <summary>
        /// Stops observing for the model and view registration at the GameUI Manager and
        /// detaches if it was previously detached
        /// </summary>
        protected void StopObserving()
        {
            if(_model != null && _view != null)
                OnDetach();

            GameUIManager.Instance.onModelAdded -= ObserveModelAdded;
            GameUIManager.Instance.onViewAdded -= ObserveViewAdded;

            GameUIManager.Instance.onModelRemoved -= ObserveModelRemoved;
            GameUIManager.Instance.onViewRemoved -= ObserveViewRemoved;
        }

        private void ObserveViewAdded(IGameView view)
        {
            if(view.GetType() != typeof(TView))
                return;

            _view = view as TView;

            if(_model != null && _view != null)
                OnAttach();
        }

        private void ObserveViewRemoved(IGameView view)
        {
            if(view.GetType() != typeof(TView))
                return;
            
            _view = null;
            OnDetach();
        }

        private void ObserveModelAdded(IGameModel model)
        {
            if(model.GetType() != typeof(TModel))
                return;

            _model = model as TModel;

            if(_model != null && _view != null)
                OnAttach();
        }

        private void ObserveModelRemoved(IGameModel model)
        {
            if(model.GetType() != typeof(TModel))
                return;

            _model = null;
            OnDetach();
        }
        #endregion
    }
}