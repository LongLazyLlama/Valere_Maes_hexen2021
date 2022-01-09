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

        private Board<Piece<TPosition>, TPosition> _board;
        private HexGrid<TPosition> _hexgrid;

        public MoveManager(Board<Piece<TPosition>, TPosition> board, HexGrid<TPosition> grid,
            int gridSize, Piece<TPosition> playerPiece)
        {
            _board = board;
            _hexgrid = grid;
            _maxSteps = gridSize * 2;

            //subscribes to the board events.
            _board.PieceMoved += (s, e) => e.Piece.MoveTo(e.ToPosition);
            _board.PiecePlaced += (s, e) => e.Piece.PlaceAt(e.AtPosition);
            _board.PieceTaken += (s, e) => e.Piece.TakeFrom(e.FromPosition);

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
                new ConfigurableMove<TPosition>(board, grid, (b, g, p)
                => new PositionHelper<TPosition>(b, g, p)
                        .Any()
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

            Debug.Log($"Moveset dictionary now contains {_moves.Count} movesets.");

            if (_moves.TryGetValue(CardType.Swipe, out var moves))
            {
                var movecount = ValidPositionsFor(playerPiece).Count;

                Debug.Log($"Swipe moveset now contains {movecount} moves.");
            }

            //_moves.Add(PieceType.Pawn, new PawnMove(board, grid));
            //_moves.Add(PieceType.Pawn, new PawnDoubleMove());
        }

        //Returns a list of valid positions for a Card.
        public List<TPosition> ValidPositionsFor(Piece<TPosition> piece)
        {
            var result = _moves[CardType.Swipe]
                        .Where((m) => m.CanExecute(piece))
                        .SelectMany((m) => m.Positions(piece))
                        .ToList();

            return result;
        }

        public void Move(Piece<TPosition> piece, TPosition position)
        {
            //neemt alle mogelijke moves.
            var move = _moves[CardType.Swipe]
                .Where(m => m.CanExecute(piece))
                .Where(m => m.Positions(piece).Contains(position))
                .First();

            move.Execute(piece, position);
            //get first moves
            //of which position is part of validmoves.
            //execute.
        }
    }
}
