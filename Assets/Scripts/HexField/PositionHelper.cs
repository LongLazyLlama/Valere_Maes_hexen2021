using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexField
{
    internal class PositionHelper<TPosition>
    {
        private List<TPosition> _validPositions = new List<TPosition>();

        //First constructor takes the base info needed to operate the movementhelper.
        public PositionHelper()
        {

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
        //public PositionHelper<TPosition> NorthEast(int maxRange = int.MaxValue, params Validator[] validators)
        //   => Collect(1, -1, 0, maxRange, validators);
        //public PositionHelper<TPosition> East(int maxRange = int.MaxValue, params Validator[] validators)
        //   => Collect(1, 0, -1, maxRange, validators);
        //public PositionHelper<TPosition> SouthEast(int maxRange = int.MaxValue, params Validator[] validators)
        //   => Collect(0, 1, -1, maxRange, validators);
        //public PositionHelper<TPosition> SouthWest(int maxRange = int.MaxValue, params Validator[] validators)
        //   => Collect(-1, 1, 0, maxRange, validators);
        //public PositionHelper<TPosition> West(int maxRange = int.MaxValue, params Validator[] validators)
        //   => Collect(-1, 0, 1, maxRange, validators);
        //public PositionHelper<TPosition> NorthWest(int maxRange = int.MaxValue, params Validator[] validators)
        //   => Collect(0, -1, 1, maxRange, validators);

        ////Takes any valid position in range
        //public PositionHelper<TPosition> Any(int maxRange = int.MaxValue, params Validator[] validators);

        ////Checks if required info is present
        //public delegate bool Validator(Board<Piece<TPosition>, TPosition> board, Grid<TPosition> grid, Piece<TPosition> piece, TPosition toPosition);

        ////Third constructor 
        //public PositionHelper<TPosition> Collect(int v, int a, int l, int maxRange, Validator[] validators)
        //{
        //    throw new System.NotImplementedException();
        //}

        public List<TPosition> CollectValidPositions()
        {
            return _validPositions;
        }

        public int Distance(Vector3 cubeStart, Vector3 cubeEnd)
        {
            var difference = cubeStart - cubeEnd;
            var distance = Mathf.RoundToInt(Mathf.Max(Mathf.Abs(difference.x), Mathf.Abs(difference.y), Mathf.Abs(difference.z)));

            Debug.Log($"Distance between {cubeStart} and {cubeEnd} is: {distance}");

            return distance;
        }

        //public static bool Empty(HexField<Unit<TPosition>, TPosition> board, Grid<TPosition> grid, Piece<TPosition> piece, TPosition toPosition)
            //=> !board.TryGetPiece(toPosition, out var _);

        //public static bool ContainsEnemy(Board<Piece<TPosition>, TPosition> board, Grid<TPosition> grid, Piece<TPosition> piece, TPosition toPosition)
            //=> board.TryGetPiece(toPosition, out var toPiece) && toPiece.PlayerID != piece.PlayerID;

        //public Vector3 GetPlayerPosition(Vector3 playerCartesianCoord)
        //{
        //    return HexGrid.CartesianCoordinatesToCube(playerCartesianCoord);
        //}


        //var playerStartWorldPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        //var playerCurrentWorldPos = CartesianCoordinate;

        //var playerCubeStartPos = _positionHelper.HexGrid.CartesianCoordinatesToCube(playerStartWorldPos);
        //var playerCubeCurrentPos = _positionHelper.HexGrid.CartesianCoordinatesToCube(playerCurrentWorldPos);

        //    if (_positionHelper.Distance(playerCubeStartPos, playerCubeCurrentPos) <= 1)
        //    {
        //        GameObject.FindGameObjectWithTag("Player").transform.position = CartesianCoordinate;
        //    }
    }
}
