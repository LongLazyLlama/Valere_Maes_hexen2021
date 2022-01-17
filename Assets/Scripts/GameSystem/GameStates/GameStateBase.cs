using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HexSystem;
using GameSystem;
using DAE.StateSystem;

namespace GameSystem.GameStates
{
    abstract class GameStateBase : IState<GameStateBase>
    {
        public const string PlayingState = "playing";
        public const string ReplayingState = "replaying";

        public StateMachine<GameStateBase> StateMachine => _stateMachine;

        private StateMachine<GameStateBase> _stateMachine;

        protected GameStateBase(StateMachine<GameStateBase> stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public virtual void OnEnter()
        {
            throw new NotImplementedException();
        }

        public virtual void OnExit()
        {
            throw new NotImplementedException();
        }
    }
}
