using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexSystem;
using BoardSystem;
using CardSystem;
using System;

namespace GameSystem
{
    public class GameLoop : MonoBehaviour
    {
        [SerializeField]
        private GameObject _hexPrefab;
        [SerializeField]
        private CoordinateConverter _coordinateConverter;

        public static GameLoop gameLoop;

        private Piece<Hex> _playerPiece;
        private MoveManager<Hex> _moveManager;
        private SelectionManager<Piece<Hex>> _selectionManager;

        [SerializeField]
        private int _currentPlayerID = 1;
        [SerializeField]
        private float _hexSize = 2.0f;
        [SerializeField][Range(1, 20)]
        private int _gridSize = 3;

        private Vector3 _playerPos;

        private List<Vector3> HexGridData = new List<Vector3>();

        public void Start()
        {
            if (gameLoop == null)
            {
                gameLoop = this;
            }

            var board = new Board<Piece<Hex>, Hex>();
            var grid = new HexGrid<Hex>(_gridSize);

            GenerateHexField(grid);
            ConnectPiece(board, grid);
            GetPlayerPiece(board, grid, out var playerPiece);

            _playerPiece = playerPiece;
            _selectionManager = new SelectionManager<Piece<Hex>>();
            _moveManager = new MoveManager<Hex>(board, grid, _gridSize, playerPiece);
        }

        private void GenerateHexField(HexGrid<Hex> grid)
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

                var (v, a, l) = _coordinateConverter.CartesianCoordinatesToCube(pieceView.transform.position, _hexSize);

                //Place the pawn on the tile (adds the piece with the hex to the dictionairy).
                if (grid.TryGetPositionAt(v, a, l, out Hex hex))
                {
                    board.Place(piece, hex);
                }
            }
        }

        private void GetPlayerPiece(Board<Piece<Hex>, Hex> board, HexGrid<Hex> grid, out Piece<Hex> playerPiece)
        {
            //var playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;

            var pieces = FindObjectsOfType<PieceView>();
            foreach (PieceView piece in pieces)
            {
                if (piece.PlayerID == _currentPlayerID)
                {
                    _playerPos = piece.transform.position;
                }
            }

            var playerCubePos = _coordinateConverter.CartesianCoordinatesToCube(_playerPos, _hexSize);

            if (grid.TryGetPositionAt(playerCubePos.v, playerCubePos.a, playerCubePos.l, out Hex hex))
            {
                board.TryGetPiece(hex, out var piece);
                playerPiece = piece;

                Debug.Log($"Selected piece player ID: {piece.PlayerID} on worldposition {_playerPos} and cubecoordinate {playerCubePos}");
            }
            else
            {
                playerPiece = null;
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

        public void SelectValidPositions(CardType cardType)
            => SelectValidPositions(_playerPiece, cardType);
        public void DeselectValidPositions(CardType cardType)
            => DeselectValidPositions(_playerPiece, cardType);

        public void SelectIsolated(CardType cardType, Hex hex)
            => SelectIsolated(_playerPiece, cardType, hex);
        public void DeselectIsolated(CardType cardType, Hex hex)
            => DeselectIsolated(_playerPiece, cardType, hex);

        private void SelectValidPositions(Piece<Hex> piece, CardType cardtype)
        {
            var hexes = _moveManager.ValidPositionsFor(piece, cardtype);
            foreach (var hex in hexes)
                hex.Highlight = true;
        }
        private void DeselectValidPositions(Piece<Hex> piece, CardType cardtype)
        {
            var hexes = _moveManager.ValidPositionsFor(piece, cardtype);
            foreach (var hex in hexes)
                hex.Highlight = false;
        }

        private void SelectIsolated(Piece<Hex> piece, CardType cardtype, Hex hex)
        {
            //Reselect all isolated tiles.
            var hexes = _moveManager.IsolatedPositionsFor(piece, cardtype, hex);
            foreach (var h in hexes)
                h.Highlight = true;
        }
        private void DeselectIsolated(Piece<Hex> piece, CardType cardtype, Hex hex)
        {
            //Reselect all isolated tiles.
            var hexes = _moveManager.IsolatedPositionsFor(piece, cardtype, hex);
            foreach (var h in hexes)
                h.Highlight = false;
        }
    }
}