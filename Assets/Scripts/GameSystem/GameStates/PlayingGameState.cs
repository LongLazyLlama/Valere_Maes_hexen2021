//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using BoardSystem;
//using HexSystem;
//using StateSystem;

//namespace GameSystem.GameStates
//{
//    class PlayingGameState : GameStateBase
//    {
//        private Board<Piece<Hex>, Tile> _board;
//        private Grid<Tile> _grid;

//        private MoveManager<Tile> _moveManager;

//        private int _currentPlayerID = 0;

//        public PlayingGameState(StateMachine<GameStateBase> stateMachine, Board<Piece<Tile>, Tile> board, Grid<Tile> grid) : base(stateMachine)
//        {
//            _board = board;
//            _grid = grid;

//            _moveManager = new MoveManager<Tile>(_board, _grid, replayManager);
//        }

//        public override void OnEnter()
//        {
//            //subscribe to event.
//        }
//        public override void OnExit()
//        {
//            //unsubscribe to event.
//        }

//    }
//}
