using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HexSystem;
using GameSystem;
using StateSystem;
using CardSystem;
using BoardSystem;

namespace GameSystem.GameStates
{
    abstract class GameStateBase : IState<GameStateBase>
    {
        public const string FirstPlayerGameState = "firstPlayer";
        public const string SecondPlayerGameState = "secondPlayer";

        public StateMachine<GameStateBase> StateMachine => _stateMachine;

        private StateMachine<GameStateBase> _stateMachine;

        protected GameStateBase(StateMachine<GameStateBase> stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public virtual void OnEnter() { }
        public virtual void OnExit() { }

        public virtual void Forward() { }

        public virtual void SelectValidPositions(CardType cardType) { }
        public virtual void DeselectValidPositions(CardType cardType) { }
        public virtual void SelectIsolated(CardType cardType, Hex hex) { }
        public virtual void DeselectIsolated() { }
        public virtual void ExecuteCard(CardType cardType, Hex hex) { }
    }
}
