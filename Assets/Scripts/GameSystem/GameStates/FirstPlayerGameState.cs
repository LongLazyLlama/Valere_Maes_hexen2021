using BoardSystem;
using CardSystem;
using HexSystem;
using StateSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameSystem.GameStates
{
    class FirstPlayerGameState : GameStateBase
    {
        private Board<Piece<Hex>, Hex> _board;
        private HexGrid<Hex> _grid;

        private MoveManager<Hex> _moveManager;
        private Piece<Hex> _playerOnePiece;

        private GameObject _playerOneHand;

        public FirstPlayerGameState(StateMachine<GameStateBase> stateMachine, 
            Board<Piece<Hex>, Hex> board, HexGrid<Hex> grid, int gridSize, Piece<Hex> playerPiece, 
            GameObject playerHand) : base(stateMachine)
        {
            _playerOnePiece = playerPiece;
            _playerOneHand = playerHand;

            _board = board;
            _grid = grid;

            _moveManager = new MoveManager<Hex>(_board, _grid, gridSize, _playerOnePiece);
        }

        public override void SelectValidPositions(CardType cardtype)
        {
            var hexes = _moveManager.ValidPositionsFor(_playerOnePiece, cardtype);
            foreach (var hex in hexes)
                hex.Highlight = true;
        }
        public override void DeselectValidPositions(CardType cardtype)
        {
            var hexes = _moveManager.ValidPositionsFor(_playerOnePiece, cardtype);
            foreach (var hex in hexes)
                hex.Highlight = false;
        }

        public override void SelectIsolated(CardType cardtype, Hex hex)
        {
            //Reselect all isolated tiles.
            var hexes = _moveManager.IsolatedPositionsFor(_playerOnePiece, cardtype, hex);
            foreach (var h in hexes)
                if (h != null)
                    h.Highlight = true;
        }
        public override void DeselectIsolated()
        {
            //Reselect all isolated tiles.
            var hexes = _moveManager._isolatedHexes;
            if (hexes != null)
                foreach (var h in hexes)
                    if (h != null)
                        h.Highlight = false;
        }

        public override void ExecuteCard(CardType cardType, Hex hex)
        {
            _moveManager.ExecuteCard(_playerOnePiece, hex, cardType);
        }

        public override void OnEnter()
        {
            _playerOneHand.SetActive(true);
        }
        public override void OnExit()
        {
            _playerOneHand.SetActive(false);
        }

        public override void Forward()
        {
            StateMachine.MoveTo(SecondPlayerGameState);
        }
    }
}
