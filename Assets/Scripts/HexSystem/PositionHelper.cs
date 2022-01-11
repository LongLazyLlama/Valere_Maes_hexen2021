using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BoardSystem;
using System;

namespace HexSystem
{
    internal class PositionHelper<TPosition>
        where TPosition : IPosition
    {
        private Board<Piece<TPosition>, TPosition> _board;
        private HexGrid<TPosition> _hexGrid;
        private Piece<TPosition> _piece;

        private List<TPosition> _validPositions = new List<TPosition>();
        private List<TPosition> _isolatedPositions = new List<TPosition>();

        private bool _isolatedDirection;

        //First constructor takes the base info needed to operate the movementhelper.
        public PositionHelper(Board<Piece<TPosition>, TPosition> board, HexGrid<TPosition> hexGrid, 
            Piece<TPosition> piece)
        {
            _board = board;
            _hexGrid = hexGrid;
            _piece = piece;
        }

        //Hex directions
        //(1, -1, 0)    NorthEast
        //(1, 0, -1)    East
        //(0, 1, -1)    SouthEast
        //(-1, 1, 0)    SouthWest
        //(-1, 0, 1)    West
        //(0, -1, 1)    NorthWest

        //Second constructor (one of these directions) checks if the info from the first constructor is present
        //and adds the direction with a maximum range.
        public PositionHelper<TPosition> NorthEast(int maxRange = int.MaxValue, 
            bool isolatedSelection = false, TPosition hex = default, params Validator[] validators)
           => Collect(1, -1, 0, isolatedSelection, hex, maxRange, validators);
        public PositionHelper<TPosition> East(int maxRange = int.MaxValue, 
            bool isolatedSelection = false, TPosition hex = default, params Validator[] validators)
           => Collect(1, 0, -1, isolatedSelection, hex, maxRange, validators);
        public PositionHelper<TPosition> SouthEast(int maxRange = int.MaxValue, 
            bool isolatedSelection = false, TPosition hex = default, params Validator[] validators)
           => Collect(0, 1, -1, isolatedSelection, hex, maxRange, validators);
        public PositionHelper<TPosition> SouthWest(int maxRange = int.MaxValue, 
            bool isolatedSelection = false, TPosition hex = default, params Validator[] validators)
           => Collect(-1, 1, 0, isolatedSelection, hex, maxRange, validators);
        public PositionHelper<TPosition> West(int maxRange = int.MaxValue, 
            bool isolatedSelection = false, TPosition hex = default, params Validator[] validators)
           => Collect(-1, 0, 1, isolatedSelection, hex, maxRange, validators);
        public PositionHelper<TPosition> NorthWest(int maxRange = int.MaxValue, 
            bool isolatedSelection = false, TPosition hex = default, params Validator[] validators)
           => Collect(0, -1, 1, isolatedSelection, hex, maxRange, validators);

        //Targets any piece on the board at any distance. (forbidden move)
        public PositionHelper<TPosition> AnyPiece(params Validator[] validators) 
        {
            foreach (var position in _hexGrid.Positions.Values)
            {
                _hexGrid.TryGetPositionAt(position.v, position.a, position.l, out TPosition hex);

                if (_board.TryGetPiece(hex, out Piece<TPosition> piece) && piece.PlayerID != 1)
                {
                    _validPositions.Add(hex);
                }
            }

            return this;
        }
        //Targets any empty hex on the board at any distance.
        public PositionHelper<TPosition> AnyEmpty(params Validator[] validators)
        {
            foreach (var position in _hexGrid.Positions.Values)
            {
                _hexGrid.TryGetPositionAt(position.v, position.a, position.l, out TPosition hex);

                if (!_board.TryGetPiece(hex, out Piece<TPosition> p))
                {
                    _validPositions.Add(hex);
                }
            }

            return this;
        }

        //Third constructor collects all positions for the selected pawn.
        public PositionHelper<TPosition> Collect(int vOffset, int aOffset, int lOffset, bool isolatedSelection, 
            TPosition mouseHexPos, int maxSteps = int.MaxValue, params Validator[] validators)
        {
            //Gets the position of the selected pawn.
            if (!_board.TryGetPosition(_piece, out var currentPosition))
                return this;

            //Gets the cubecoordinate of the selected pawn.
            if (!_hexGrid.TryGetCubeCoordinateAt(currentPosition, out var currentCoordinates))
                return this;

            int nextCoordinateV = currentCoordinates.v + vOffset;
            int nextCoordinateA = currentCoordinates.a + aOffset;
            int nextCoordinateL = currentCoordinates.l + lOffset;

            //Gets the cubecoordinate of the neigbouring hex.
            _hexGrid.TryGetPositionAt(
                currentCoordinates.v + vOffset,
                currentCoordinates.a + aOffset,
                currentCoordinates.l + lOffset,
                out TPosition nextPosition);

            //Gets all tiles in a direction according to the max amount of steps (range).
            var steps = 0;
            while (steps < maxSteps && nextPosition != null && validators.All((v) 
                => v(_board, _hexGrid, _piece, nextPosition)))
            {
                //Is it trying to collect the isolated positions or all valid positions.
                if (isolatedSelection)
                {
                    _hexGrid.TryGetCubeCoordinateAt(nextPosition, out var pp);
                    _hexGrid.TryGetCubeCoordinateAt(mouseHexPos, out var mp);

                    if (pp == mp && !_isolatedDirection)
                    {
                        //Set all values back to default.
                        steps = 0;
                        nextCoordinateV = currentCoordinates.v;
                        nextCoordinateA = currentCoordinates.a;
                        nextCoordinateL = currentCoordinates.l;

                        //The hex with the mousepointer ontop is in this direction.
                        _isolatedDirection = true;
                    }

                    if (_isolatedDirection)
                    {
                        _isolatedPositions.Add(nextPosition);
                    }
                }
                else
                {
                    _validPositions.Add(nextPosition);
                }

                nextCoordinateV += vOffset;
                nextCoordinateA += aOffset;
                nextCoordinateL += lOffset;

                _hexGrid.TryGetPositionAt(
                    nextCoordinateV,
                    nextCoordinateA,
                    nextCoordinateL,
                    out nextPosition);

                steps++;

                ////If there is a piece on that tile.
                //if (_board.TryGetPiece(nextPosition, out var nextPiece))
                //{
                //    if (nextPiece.PlayerID != _piece.PlayerID)
                //        _validPositions.Add(nextPosition);

                //    steps++;
                //}
                //else
                //{
                //    _validPositions.Add(nextPosition);

                //    nextCoordinateV += vOffset;
                //    nextCoordinateA += aOffset;
                //    nextCoordinateL += lOffset;

                //    _hexGrid.TryGetPositionAt(
                //        nextCoordinateV,
                //        nextCoordinateA,
                //        nextCoordinateL,
                //        out nextPosition);

                //    steps++;
                //}
            }

            _isolatedDirection = false;
            return this;
        }

        public List<TPosition> CollectIsolatedPositions(List<TPosition> validPositions, int maxSteps, TPosition hex)
        {
            NorthEast(maxSteps, true, hex);
            East(maxSteps, true, hex);
            SouthEast(maxSteps, true, hex);
            SouthWest(maxSteps, true, hex);
            West(maxSteps, true, hex);
            NorthWest(maxSteps, true, hex);

            return _isolatedPositions;
        }
        public List<TPosition> CollectValidPositions()
        {
            return _validPositions;
        }


        public delegate bool Validator(Board<Piece<TPosition>, TPosition> board, HexGrid<TPosition> grid, 
            Piece<TPosition> piece, TPosition toPosition);

        public static bool Empty(Board<Piece<TPosition>, TPosition> board, HexGrid<TPosition> hexGrid, Piece<TPosition> piece, TPosition toPosition)
            => !board.TryGetPiece(toPosition, out var _);
        public static bool ContainsEnemy(Board<Piece<TPosition>, TPosition> board, HexGrid<TPosition> hexGrid, Piece<TPosition> piece, TPosition toPosition)
            => board.TryGetPiece(toPosition, out var toPiece) && toPiece.PlayerID != piece.PlayerID;

        //--------------------------------------------------------------------------------------------------------------------

        //public int Distance(Vector3 cubeStart, Vector3 cubeEnd)
        //{
        //    var difference = cubeStart - cubeEnd;
        //    var distance = Mathf.RoundToInt(Mathf.Max(Mathf.Abs(difference.x), Mathf.Abs(difference.y), Mathf.Abs(difference.z)));

        //    Debug.Log($"Distance between {cubeStart} and {cubeEnd} is: {distance}");

        //    return distance;
        //}
    }
}
