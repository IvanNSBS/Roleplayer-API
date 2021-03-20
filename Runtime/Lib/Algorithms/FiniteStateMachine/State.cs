namespace Lib.Algorithms.FiniteStateMachine
{
    public class State<TEntity>
    {
        #region Properties
        public TEntity Entity { get; set; }
        public FSM<TEntity> FSM { get; set; }
        #endregion Properties


        #region Methods
        public virtual void Update() { }
        public virtual void Enter() { }
        public virtual void Exit() { }
        #endregion Methods
    }
}