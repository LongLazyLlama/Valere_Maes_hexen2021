using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexSystem;
using BoardSystem;

namespace GameSystem
{
    public class GameLoop : MonoBehaviour
    {
        [SerializeField]
        private GameObject _hexPrefab;
        [SerializeField]
        private CoordinateConverter _coordinateConverter;

        [SerializeField]
        private float _hexSize = 2.0f;
        [SerializeField][Range(1, 20)]
        private int _gridSize = 3;

        private List<Vector3> HexGridData = new List<Vector3>();
        private MoveManager<Hex> _moveManager;

        public void Start()
        {
            var board = new Board<Piece<Hex>, Hex>();
            var grid = new HexGrid<Hex>(_gridSize);

            GenerateHexField(grid);
            ConnectPiece(board, grid);

            //_moveManager = new MoveManager<Hex>(board, grid);
        }

        public void GenerateHexField(HexGrid<Hex> grid)
        {
            //for gridsize create a cubecoordinate for each hex.
            //put them in a list
            HexGridData = grid.GenerateCubeCoordinates();

            //for each cubecoodinate in the list => convert to cartesian coordinates
            foreach (Vector3 cubeCoordinate in HexGridData)
            {
                //Converts the cubeCoordinate to cartesian.
                var cartesianCoordinate = _coordinateConverter.CubeCoordinatesToCartesian(cubeCoordinate, _hexSize);
                cartesianCoordinate += this.transform.position;

                //instantiate a hex on every cartesian coordinate and connects it to its coordinate.
                CreateHex(cartesianCoordinate, out Hex hex);
                ConnectHex(grid, cubeCoordinate, hex);
            }
        }

        private void ConnectHex(HexGrid<Hex> grid, Vector3 cubeCoordinate, Hex hex)
        {
            //tile.Clicked += (s, e) => Select(e.Tile);

            //Registers the hex.
            grid.Register(hex, (int)cubeCoordinate.x, (int)cubeCoordinate.y, (int)cubeCoordinate.z);
        }

        private void ConnectPiece(Board<Piece<Hex>, Hex> board, HexGrid<Hex> grid)
        {
            var pieceViews = FindObjectsOfType<PieceView>();
            foreach (var pieceView in pieceViews)
            {
                var piece = new Piece<Hex>();

                piece.PlayerID = pieceView.PlayerID;
                pieceView.Model = piece;

                var (v, a, l) = _coordinateConverter.CartesianCoordinatesToCube(pieceView.transform.localPosition, _hexSize);

                //Place the pawn on the tile (adds the piece with the hex to the dictionairy).
                if (grid.TryGetPositionAt(v, a, l, out Hex hex))
                {
                    board.Place(piece, hex);
                }

                //Subscribes the pieceview to select so it can be selected.
                pieceView.Clicked += (s, e) => Select(e.Piece);
            }
        }

        private void DeleteHexField()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        private void CreateHex(Vector3 cartesianCoordinate, out Hex hex)
        {
            var hexObject = Instantiate(_hexPrefab, cartesianCoordinate, _hexPrefab.transform.rotation, transform);
            hex = hexObject.GetComponentInChildren<Hex>();
        }

        private void Select(Piece<Hex> piece)
            => _gameStateMachine.CurrentState.Select(piece);
        private void Select(Hex hex)
            => _gameStateMachine.CurrentState.Select(hex);


        //private void Select(Piece<Tile> piece)
        //    => _gameStateMachine.CurrentState.Select(piece);
        //private void Select(Tile tile)
        //    => _gameStateMachine.CurrentState.Select(tile);
    }
}