using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAE.StateSystem
{
    public interface IState<TState>
        where TState : IState<TState>
    {
        void OnEnter();
        void OnExit();

        StateMachine<TState> StateMachine { get; }
    }
}
