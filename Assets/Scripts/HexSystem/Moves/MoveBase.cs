using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoardSystem;
using CardSystem;

namespace HexSystem.Moves
{
    abstract class MoveBase<TPosition> : IMove<TPosition>
        where TPosition : IPosition
    {
        protected Board<Piece<TPosition>, TPosition> Board { get; }
        protected HexGrid<TPosition> HexGrid { get; }

        protected MoveBase(Board<Piece<TPosition>, TPosition> board, HexGrid<TPosition> hexGrid)
        {
            Board = board;
            HexGrid = hexGrid;
        }

        //Return true to IMove.
        public bool CanExecute(Piece<TPosition> piece)
            => true;

        public void Execute(Piece<TPosition> piece, TPosition position , List<TPosition> isolatedPositions, CardType cardType)
        {
            //Takes the old position.
            if (!Board.TryGetPosition(piece, out var oldPosition))
                return;

            switch (cardType)
            {
                case CardType.Pushback :
                {
                    foreach (var pos in isolatedPositions)
                    {
                        //If there is a piece on the new position, take it.
                        var pieceTaken = Board.TryGetPiece(pos, out var toPiece);


                        //if (pieceTaken)
                        //    Board.Move(toPiece);
                    }

                    break;

                }
                case CardType.Teleport :
                {
                    Board.Move(piece, position);
                    break;
                }
                case CardType.Slash :
                {
                    foreach (var pos in isolatedPositions)
                    {
                        //If there is a piece on the new position, take it.
                        var pieceTaken = Board.TryGetPiece(pos, out var toPiece);
                        if (pieceTaken)
                            Board.Take(toPiece);
                    }
                    break;
                }
                case CardType.Swipe :
                {
                    foreach (var pos in isolatedPositions)
                    {
                        //If there is a piece on the new position, take it.
                        var pieceTaken = Board.TryGetPiece(pos, out var toPiece);
                        if (pieceTaken)
                            Board.Take(toPiece);
                    }
                    break;
                }
            }
        }

        public abstract List<TPosition> Positions(Piece<TPosition> piece);
    }
}
