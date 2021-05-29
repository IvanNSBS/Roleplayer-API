using System;
using System.Collections.Generic;

namespace Lib.Algorithms.FiniteStateMachine
{
    public class FSM<TEntity>
    {
        #region Fields
        protected TEntity entity;
        protected State<TEntity> currentState;
        protected State<TEntity> previousState;

        protected Dictionary<Type, List<Transition>> transitions = new Dictionary<Type, List<Transition>>();
        protected List<Transition> currentTransitions = new List<Transition>();
        protected List<Transition> anyTransitions = new List<Transition>();
        #endregion Fields

        #region Static Fields
        private static List<Transition> EmptyTransitions = new List<Transition>(0);
        #endregion Static Feilds

        #region Properties
        public State<TEntity> CurrentState { get => currentState; }
        public State<TEntity> PreviousState { get => previousState; }
        #endregion Properties


        #region Constructors
        public FSM(TEntity entity)
        {
            this.entity = entity;
        }
        #endregion Constructors


        #region Methods
        public void Update()
        {
            var transition = GetTransition();
            if (transition != null)
                ChangeState(transition.To);

            currentState?.Update();
        }

        public void ChangeState(State<TEntity> state)
        {
            if (state == currentState)
                return;

            currentState?.Exit();
            previousState = currentState;
            currentState = state;
            
            currentState.Entity = entity;
            currentState.FSM = this;
            currentState.Enter();

            transitions.TryGetValue(currentState.GetType(), out currentTransitions);
            if (currentTransitions == null)
                currentTransitions = EmptyTransitions;
        }
        public void AddTransition(State<TEntity> from, State<TEntity> to, Func<bool> predicate)
        {
            if (transitions.TryGetValue(from.GetType(), out var newTransitions) == false)
            {
                newTransitions = new List<Transition>();
                transitions[from.GetType()] = newTransitions;
            }

            newTransitions.Add(new Transition(to, predicate));
        }

        public void AddAnyTransition(State<TEntity> state, Func<bool> predicate)
        {
            anyTransitions.Add(new Transition(state, predicate));
        }

        public string CurrentStateName()
        {
            return currentState.GetType().Name;
        }

        private Transition GetTransition()
        {
            foreach (var transition in anyTransitions)
                if (transition.Condition())
                    return transition;

            foreach (var transition in currentTransitions)
                if (transition.Condition())
                    return transition;

            return null;
        }
        #endregion Methods

        #region Transition Class
        protected class Transition
        {
            public Func<bool> Condition { get; }
            public State<TEntity> To { get; }

            public Transition(State<TEntity> to, Func<bool> condition)
            {
                To = to;
                Condition = condition;
            }
        }

        #endregion Transition Class
    }
}