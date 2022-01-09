using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoardSystem;

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

        public void Execute(Piece<TPosition> piece, TPosition position)
        {
            //zoekt de old position.
            if (Board.TryGetPosition(piece, out var oldPosition))
                return;

            var pieceTaken = Board.TryGetPiece(position, out var toPiece);

            //Lambda's houden alle data in een object uit de stack.
            Action forward = () =>
            {
                //is there a piece on position
                if (pieceTaken)
                    //take piece
                    Board.Take(toPiece);


                //move to position
                Board.Move(piece, position);
            };

            Action backward = () =>
            {
                Board.Move(piece, oldPosition);
            };
        }

        public abstract List<TPosition> Positions(Piece<TPosition> piece);
    }
}
