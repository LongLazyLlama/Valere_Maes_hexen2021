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
    class SecondPlayerGameState : GameStateBase
    {
        private Board<Piece<Hex>, Hex> _board;
        private HexGrid<Hex> _grid;

        private MoveManager<Hex> _moveManager;
        private Piece<Hex> _playerTwoPiece;

        private GameObject _playerTwoHand;

        public SecondPlayerGameState(StateMachine<GameStateBase> stateMachine,
            Board<Piece<Hex>, Hex> board, HexGrid<Hex> grid, int gridSize, Piece<Hex> playerPiece,
            GameObject playerHand) : base(stateMachine)
        {
            _playerTwoPiece = playerPiece;
            _playerTwoHand = playerHand;

            _board = board;
            _grid = grid;

            _moveManager = new MoveManager<Hex>(_board, _grid, gridSize, _playerTwoPiece);
        }

        public override void SelectValidPositions(CardType cardtype)
        {
            var hexes = _moveManager.ValidPositionsFor(_playerTwoPiece, cardtype);
            foreach (var hex in hexes)
                hex.Highlight = true;
        }
        public override void DeselectValidPositions(CardType cardtype)
        {
            var hexes = _moveManager.ValidPositionsFor(_playerTwoPiece, cardtype);
            foreach (var hex in hexes)
                hex.Highlight = false;
        }

        public override void SelectIsolated(CardType cardtype, Hex hex)
        {
            //Reselect all isolated tiles.
            var hexes = _moveManager.IsolatedPositionsFor(_playerTwoPiece, cardtype, hex);
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
            _moveManager.ExecuteCard(_playerTwoPiece, hex, cardType);
        }

        public override void OnEnter()
        {
            _playerTwoHand.SetActive(true);
        }
        public override void OnExit()
        {
            _playerTwoHand.SetActive(false);
        }

        public override void Forward()
        {
            StateMachine.MoveTo(FirstPlayerGameState);
        }
    }
}
