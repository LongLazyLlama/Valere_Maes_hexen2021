using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexSystem;
using BoardSystem;
using CardSystem;
using System;
using Random = UnityEngine.Random;

namespace GameSystem
{
    public class GameLoop : MonoBehaviour
    {
        [SerializeField]
        private GameObject _hexPrefab;
        [SerializeField]
        private GameObject _enemyPrefab;
        [SerializeField]
        private GameObject _playerPrefab;
        [SerializeField]
        private CoordinateConverter _coordinateConverter;

        public static GameLoop gameLoop;

        private Piece<Hex> _playerPiece;
        private MoveManager<Hex> _moveManager;

        [Space]
        [SerializeField]
        private bool _spawnPlayerRandomly;

        [Space]
        [SerializeField]
        private int _currentPlayerID = 1;
        [SerializeField][Range(1, 50)]
        private int _enemyCount = 10;
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

            DeleteHexField();
            DeletePieces();

            GenerateHexField(grid);
            GeneratePieces(grid);

            ConnectPiece(board, grid);
            GetPlayerPiece(board, grid, out var playerPiece);

            _playerPiece = playerPiece;
            _moveManager = new MoveManager<Hex>(board, grid, _gridSize, playerPiece);
        }

        private void DeleteHexField()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
        private void DeletePieces()
        {
            var pieces = FindObjectsOfType<PieceView>();
            foreach (PieceView piece in pieces)
            {
                Destroy(piece.gameObject);
            }
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
        private void GeneratePieces(HexGrid<Hex> grid)
        {
            List<int> usedPositions = new List<int>();

            if (_spawnPlayerRandomly)
            {
                var randomPlayerPos = Random.Range(0, grid.CubeCoordinates.Count);
                var playerpos = _coordinateConverter.CubeCoordinatesToCartesian
                        (grid.CubeCoordinates[randomPlayerPos], _hexSize);

                Instantiate(_playerPrefab, playerpos, Quaternion.identity);
                _playerPos = playerpos;

                usedPositions.Add(randomPlayerPos);
            }
            else
            {
                int playerpos = grid.CubeCoordinates.IndexOf(new Vector3(0, 0, 0));

                Instantiate(_playerPrefab, Vector3.zero, Quaternion.identity);
                _playerPos = Vector3.zero;

                usedPositions.Add(playerpos);
            }

            if (_enemyCount > grid.CubeCoordinates.Count)
                _enemyCount = grid.CubeCoordinates.Count - usedPositions.Count;

            for (int i = 0; i < _enemyCount; i++)
            {
                var randomEnemyPos = Random.Range(0, grid.CubeCoordinates.Count);

                if (!usedPositions.Contains(randomEnemyPos))
                {
                    var worldPos = _coordinateConverter.CubeCoordinatesToCartesian
                        (grid.CubeCoordinates[randomEnemyPos], _hexSize);

                    Instantiate(_enemyPrefab, worldPos, Quaternion.identity);
                    usedPositions.Add(randomEnemyPos);
                }
                else
                    i--;
            }
        }

        private void ConnectHex(HexGrid<Hex> grid, Vector3 cubeCoordinate, Hex hex)
        {
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
            var playerCubePos = _coordinateConverter.CartesianCoordinatesToCube(_playerPos, _hexSize);

            if (grid.TryGetPositionAt(playerCubePos.v, playerCubePos.a, playerCubePos.l, out Hex hex))
            {
                board.TryGetPiece(hex, out var piece);
                playerPiece = piece;

                //Debug.Log($"Selected piece player ID: {piece.PlayerID} on worldposition {_playerPos} and cubecoordinate {playerCubePos}");
            }
            else
            {
                playerPiece = null;
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
        public void ExecuteCard(CardType cardType, Hex hex)
            => _moveManager.ExecuteCard(_playerPiece, hex, cardType);

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
                if (h != null)
                    h.Highlight = true;
        }
        public void DeselectIsolated()
        {
            //Reselect all isolated tiles.
            var hexes = _moveManager._isolatedHexes;
            if (hexes != null)
                foreach (var h in hexes)
                    if (h != null)
                        h.Highlight = false;
        }
    }
}