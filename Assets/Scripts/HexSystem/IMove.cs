using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexSystem
{
    interface IMove<TPosition>
        where TPosition : IPosition
    {
        bool CanExecute(Piece<TPosition> piece);

        void Execute(Piece<TPosition> piece, TPosition position);

        List<TPosition> Positions(Piece<TPosition> piece);
    }
}
