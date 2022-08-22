using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateSystem
{
    public class StateMachine<TState>
        where TState : IState<TState>
    {
        private Dictionary<string, TState> _states = new Dictionary<string, TState>();

        public TState CurrentState => _states[_currentStateName];

        private string _currentStateName;

        public string InitialState
        {
            set
            {
                _currentStateName = value;
                CurrentState.OnEnter();
            }
        }

        public void Register(string stateName, TState state)
        {
            _states.Add(stateName, state);
        }

        public void MoveTo(string nextState)
        {
            CurrentState?.OnExit();

            _currentStateName = nextState;

            CurrentState?.OnEnter();
        }
    }
}
