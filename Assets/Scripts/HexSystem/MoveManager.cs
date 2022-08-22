using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CardSystem;
using BoardSystem;
using DAE.Commons;
using HexSystem.Moves;
using UnityEngine;

namespace HexSystem
{
    public class MoveManager<TPosition>
        where TPosition : IPosition
    {
        private int _maxSteps;

        //Stores Movesets (a card type with its possible moves)
        private MultiValueDictionary<CardType, IMove<TPosition>> _moves = 
            new MultiValueDictionary<CardType, IMove<TPosition>>();

        public List<TPosition> _isolatedHexes = default;
        public List<TPosition> _targetHexes = default;

        private Board<Piece<TPosition>, TPosition> _board;
        private HexGrid<TPosition> _hexgrid;
        private Piece<TPosition> _playerPiece;

        public MoveManager(Board<Piece<TPosition>, TPosition> board, HexGrid<TPosition> grid,
            int gridSize, Piece<TPosition> playerPiece)
        {
            _board = board;
            _hexgrid = grid;
            _maxSteps = gridSize * 2;
            _playerPiece = playerPiece;

            //subscribes to the piece events.
            _board.PieceMoved += (s, e) => e.Piece.MoveTo(e.ToPosition);
            _board.PiecePlaced += (s, e) => e.Piece.PlaceAt(e.AtPosition);
            _board.PieceTaken += (s, e) => e.Piece.TakeFrom(e.FromPosition);

            //_hexgrid.HexRemoved += (s, e) => ;

            //Creates the movesets for each card.
            _moves.Add(CardType.Swipe,
                new ConfigurableMove<TPosition>(board, grid, (b, h, p) 
                => new PositionHelper<TPosition>(b, h, p)
                        .NorthEast(_maxSteps)
                        .East(_maxSteps)
                        .SouthEast(_maxSteps)
                        .SouthWest(_maxSteps)
                        .West(_maxSteps)
                        .NorthWest(_maxSteps)
                        .CollectValidPositions()));

            _moves.Add(CardType.Teleport,
                new ConfigurableMove<TPosition>(board, grid, (b, h, p)
                => new PositionHelper<TPosition>(b, h, p)
                        .AnyEmpty()
                        .CollectValidPositions()));

            _moves.Add(CardType.Slash,
                new ConfigurableMove<TPosition>(board, grid, (b, h, p)
                => new PositionHelper<TPosition>(b, h, p)
                        .NorthEast(1)
                        .East(1)
                        .SouthEast(1)
                        .SouthWest(1)
                        .West(1)
                        .NorthWest(1)
                        .CollectValidPositions()));

            _moves.Add(CardType.Pushback,
                new ConfigurableMove<TPosition>(board, grid, (b, h, p)
                => new PositionHelper<TPosition>(b, h, p)
                        .NorthEast(1)
                        .East(1)
                        .SouthEast(1)
                        .SouthWest(1)
                        .West(1)
                        .NorthWest(1)
                        .CollectValidPositions()));

            //Debug.Log($"Moveset dictionary now contains {_moves.Count} movesets.");

            //if (_moves.TryGetValue(CardType.Swipe, out var moves))
            //{
            //    var movecount = ValidPositionsFor(playerPiece, CardType.Swipe).Count;

            //    //Debug.Log($"Swipe moveset now contains {movecount} moves.");
            //}
        }

        //Returns a list of valid positions for a Card.
        public List<TPosition> ValidPositionsFor(Piece<TPosition> piece, CardType cardType)
        {
            var result = _moves[cardType]
                        .Where((m) => m.CanExecute(piece))
                        .SelectMany((m) => m.Positions(piece))
                        .ToList();

            return result;
        }

        public List<TPosition> IsolatedPositionsFor(Piece<TPosition> piece, CardType cardType, TPosition mousePosHex)
        {
            //Take all previously highlighted hexes.
            var validHexes = ValidPositionsFor(piece, cardType);
            var maxSteps = GetMaxRange(validHexes);

            if (cardType == CardType.Teleport)
            {
                validHexes.Clear();
                validHexes.Add(mousePosHex);

                return validHexes;
            }
            else if (cardType == CardType.Slash || cardType == CardType.Pushback)
            {
                (_isolatedHexes, _targetHexes) = new PositionHelper<TPosition>(_board, _hexgrid, piece)
                    .CollectIsolatedPositions(maxSteps, mousePosHex, true, false);
            }
            else
            {
                //Find all hexes in the direction of the card in comparison to the player.
                _isolatedHexes = new PositionHelper<TPosition>(_board, _hexgrid, piece)
                    .CollectIsolatedPositions(maxSteps, mousePosHex, false, false).Item1;
            }

            //Debug.Log($"Card of cardtype {cardType} has currently isolated {_isolatedHexes.Count} hexes.");

            return _isolatedHexes;
        }

        private int GetMaxRange(List<TPosition> positions)
        {
            //Kind of a cheat workaround but it works :)
            List<int> values = new List<int>();

            foreach (var hex in positions)
            {
                _hexgrid.TryGetCubeCoordinateAt(hex, out var cc);
                _board.TryGetPosition(_playerPiece, out var pos);
                _hexgrid.TryGetCubeCoordinateAt(pos, out var pp);

                var stepvalue = (cc.v - pp.v, cc.a - pp.a, cc.l - pp.l);
                var highestValue = Mathf.Max(stepvalue.Item1, stepvalue.Item2, stepvalue.Item3);

                values.Add(highestValue);
            }

            var maxRange = Mathf.Max(values.ToArray());

            //Debug.Log($"The max range of {cardType} is: {maxRange}");

            return maxRange;
        }

        public void ExecuteCard(Piece<TPosition> piece, TPosition position, CardType cardType)
        {
            //Checks if the hex you dropped a card on is within the validpositions.
            var move = _moves[cardType]
                .Where(m => m.CanExecute(piece))
                .Where(m => m.Positions(piece).Contains(position))
                .First();

            move.Execute(piece, position, _isolatedHexes, _targetHexes, cardType);
        }
    }
}
