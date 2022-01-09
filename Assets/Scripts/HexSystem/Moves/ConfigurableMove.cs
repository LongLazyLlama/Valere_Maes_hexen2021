using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoardSystem;

namespace HexSystem.Moves
{
    class ConfigurableMove<TPosition> : MoveBase<TPosition>
        where TPosition : IPosition
    {
        public delegate List<TPosition> PositionsCollector(Board<Piece<TPosition>, TPosition> board, HexGrid<TPosition> grid, Piece<TPosition> piece);

        private PositionsCollector _collectPositions;

        public ConfigurableMove(Board<Piece<TPosition>, TPosition> board, HexGrid<TPosition> grid,
            PositionsCollector positionsCollector) : base(board, grid)
        {
            _collectPositions = positionsCollector;
        }

        public override List<TPosition> Positions(Piece<TPosition> piece)
            => _collectPositions(Board, HexGrid, piece);
    }
}
