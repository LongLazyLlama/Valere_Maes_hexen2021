using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BoardSystem;
using System;
using UnityEngine;

namespace HexSystem
{
    internal class PositionHelper<TPosition>
        where TPosition : IPosition
    {
        private List<(int, int, int)> _hexDirections = new List<(int, int, int)>
        {
            //Hex neigbour directions
            (1, -1, 0),                    //NorthEast
            (1, 0, -1),                    //East
            (0, 1, -1),                    //SouthEast
            (-1, 1, 0),                    //SouthWest
            (-1, 0, 1),                    //West
            (0, -1, 1)                     //NorthWest
        };

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

        //Second constructor (one of these directions) checks if the info from the first constructor is present
        //and adds the direction with a maximum range.
        public PositionHelper<TPosition> NorthEast(int maxRange = int.MaxValue, 
            bool isolatedSelection = false, bool usesNeighbours = false, TPosition hex = default, params Validator[] validators)
           => Collect(1, -1, 0, isolatedSelection, usesNeighbours, hex, maxRange, validators);
        public PositionHelper<TPosition> East(int maxRange = int.MaxValue, 
            bool isolatedSelection = false, bool usesNeighbours = false, TPosition hex = default, params Validator[] validators)
           => Collect(1, 0, -1, isolatedSelection, usesNeighbours, hex, maxRange, validators);
        public PositionHelper<TPosition> SouthEast(int maxRange = int.MaxValue, 
            bool isolatedSelection = false, bool usesNeighbours = false, TPosition hex = default, params Validator[] validators)
           => Collect(0, 1, -1, isolatedSelection, usesNeighbours, hex, maxRange, validators);
        public PositionHelper<TPosition> SouthWest(int maxRange = int.MaxValue, 
            bool isolatedSelection = false, bool usesNeighbours = false, TPosition hex = default, params Validator[] validators)
           => Collect(-1, 1, 0, isolatedSelection, usesNeighbours, hex, maxRange, validators);
        public PositionHelper<TPosition> West(int maxRange = int.MaxValue, 
            bool isolatedSelection = false, bool usesNeighbours = false, TPosition hex = default, params Validator[] validators)
           => Collect(-1, 0, 1, isolatedSelection, usesNeighbours, hex, maxRange, validators);
        public PositionHelper<TPosition> NorthWest(int maxRange = int.MaxValue, 
            bool isolatedSelection = false, bool usesNeighbours = false, TPosition hex = default, params Validator[] validators)
           => Collect(0, -1, 1, isolatedSelection, usesNeighbours, hex, maxRange, validators);

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
        public PositionHelper<TPosition> Collect(int vOffset, int aOffset, int lOffset, bool isolatedSelection, bool usesNeighbours,
            TPosition mouseHexPos, int maxSteps = int.MaxValue, params Validator[] validators)
        {
            //Gets the position of the selected pawn.
            if (!_board.TryGetPosition(_piece, out var currentPosition))
                return this;

            //Gets the cubecoordinate of the selected pawn.
            if (!_hexGrid.TryGetCubeCoordinateAt(currentPosition, out var currentCoordinates))
                return this;

            (int, int, int) currentDirectionOffset = (vOffset, aOffset, lOffset);

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
                    _hexGrid.TryGetCubeCoordinateAt(nextPosition, out var nextPos);
                    _hexGrid.TryGetCubeCoordinateAt(mouseHexPos, out var mousePos);
                    _hexGrid.TryGetCubeCoordinateAt(currentPosition, out var playerPos);

                    if (nextPos == mousePos && !_isolatedDirection)
                    {
                        //Set all values back to default (steps -1 because it still goes up by one at the end of the loop = 0).
                        steps = -1;
                        nextCoordinateV = currentCoordinates.v;
                        nextCoordinateA = currentCoordinates.a;
                        nextCoordinateL = currentCoordinates.l;

                        //The hex with the mousepointer ontop is in this direction.
                        _isolatedDirection = true;
                    }
                    if (usesNeighbours && _isolatedDirection)
                    {
                        GetNeighbouringHexes(playerPos, currentDirectionOffset);
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
            }

            _isolatedDirection = false;
            return this;
        }

        public List<TPosition> CollectIsolatedPositions(int maxSteps, TPosition hex, bool usesNeighbours)
        {
            NorthEast(maxSteps, true, usesNeighbours, hex);
            East(maxSteps, true, usesNeighbours, hex);
            SouthEast(maxSteps, true, usesNeighbours, hex);
            SouthWest(maxSteps, true, usesNeighbours, hex);
            West(maxSteps, true, usesNeighbours, hex);
            NorthWest(maxSteps, true, usesNeighbours, hex);

            return _isolatedPositions;
        }
        public List<TPosition> CollectValidPositions()
        {
            return _validPositions;
        }

        public void GetNeighbouringHexes((int, int, int) playerPosition, (int, int, int) currentDirection)
        {
            var mouseDirIndex = _hexDirections.FindIndex((m) => m == currentDirection);

            //Get previous Neighbour.
            var previousIndex = mouseDirIndex - 1;
            if (previousIndex < 0)
                previousIndex = 5;

            var previousNeighbourV = playerPosition.Item1 + _hexDirections[previousIndex].Item1;
            var previousNeighbourA = playerPosition.Item2 + _hexDirections[previousIndex].Item2;
            var previousNeighbourL = playerPosition.Item3 + _hexDirections[previousIndex].Item3;

            _hexGrid.TryGetPositionAt(previousNeighbourV, previousNeighbourA, previousNeighbourL, out var previousNeighbour);
            _isolatedPositions.Add(previousNeighbour);

            //Get next Neighbour.
            var nextIndex = mouseDirIndex + 1;
            if (nextIndex > 5)
                nextIndex = 0;

            var nextNeighbourV = playerPosition.Item1 + _hexDirections[nextIndex].Item1;
            var nextNeighbourA = playerPosition.Item2 + _hexDirections[nextIndex].Item2;
            var nextNeighbourL = playerPosition.Item3 + _hexDirections[nextIndex].Item3;

            _hexGrid.TryGetPositionAt(nextNeighbourV, nextNeighbourA, nextNeighbourL, out var nextNeighbour);
            _isolatedPositions.Add(nextNeighbour);
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
