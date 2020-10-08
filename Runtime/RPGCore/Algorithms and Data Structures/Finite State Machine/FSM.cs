using System;
using System.Collections.Generic;

namespace RPGCore.Algorithms_and_Data_Structures.Finite_State_Machine
{
    public class FSM<TEntity>
    {
        #region Fields
        private TEntity m_entity;
        private State<TEntity> m_currentState;
        private State<TEntity> m_previousState;

        private Dictionary<Type, List<Transition>> m_transitions = new Dictionary<Type, List<Transition>>();
        private List<Transition> m_currentTransitions = new List<Transition>();
        private List<Transition> m_anyTransitions = new List<Transition>();
        #endregion Fields

        #region Static Fields
        private static List<Transition> EmptyTransitions = new List<Transition>(0);
        #endregion Static Feilds

        #region Properties
        public State<TEntity> CurrentState { get => m_currentState; }
        public State<TEntity> PreviousState { get => m_previousState; }
        #endregion Properties


        #region Constructors
        public FSM(TEntity entity)
        {
            this.m_entity = entity;
        }
        #endregion Constructors


        #region Methods
        public void Update()
        {
            var transition = GetTransition();
            if (transition != null)
                ChangeState(transition.To);

            m_currentState?.Update();
        }

        public void ChangeState(State<TEntity> state)
        {
            if (state == m_currentState)
                return;

            m_currentState?.Exit();
            m_previousState = m_currentState;
            m_currentState = state;
            
            m_currentState.Entity = m_entity;
            m_currentState.FSM = this;
            m_currentState.Enter();

            m_transitions.TryGetValue(m_currentState.GetType(), out m_currentTransitions);
            if (m_currentTransitions == null)
                m_currentTransitions = EmptyTransitions;
        }
        public void AddTransition(State<TEntity> from, State<TEntity> to, Func<bool> predicate)
        {
            if (m_transitions.TryGetValue(from.GetType(), out var newTransitions) == false)
            {
                newTransitions = new List<Transition>();
                m_transitions[from.GetType()] = newTransitions;
            }

            newTransitions.Add(new Transition(to, predicate));
        }

        public void AddAnyTransition(State<TEntity> state, Func<bool> predicate)
        {
            m_anyTransitions.Add(new Transition(state, predicate));
        }

        public string CurrentStateName()
        {
            return m_currentState.GetType().Name;
        }

        private Transition GetTransition()
        {
            foreach (var transition in m_anyTransitions)
                if (transition.Condition())
                    return transition;

            foreach (var transition in m_currentTransitions)
                if (transition.Condition())
                    return transition;

            return null;
        }
        #endregion Methods

        #region Transition Class
        private class Transition
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