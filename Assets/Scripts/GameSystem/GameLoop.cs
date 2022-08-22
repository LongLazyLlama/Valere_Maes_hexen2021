using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexSystem;
using BoardSystem;
using CardSystem;
using System;
using Random = UnityEngine.Random;
using StateSystem;
using GameSystem.GameStates;

namespace GameSystem
{
    public class GameLoop : MonoBehaviour
    {
        [SerializeField]
        private GameObject _hexPrefab = null;
        [SerializeField]
        private GameObject _enemyPrefab = null;

        [Space]
        [SerializeField]
        private GameObject _playerOnePrefab = null;
        [SerializeField]
        private GameObject _playerOneHand = null;

        [Space]
        [SerializeField]
        private GameObject _playerTwoPrefab = null;
        [SerializeField]
        private GameObject _playerTwoHand = null;

        [SerializeField]
        private CoordinateConverter _coordinateConverter = null;

        private StateMachine<GameStateBase> _gameStateMachine;

        public static GameLoop gameLoop;
        private MoveManager<Hex> _moveManager;

        private Piece<Hex> _playerPiece;

        [Space]
        [SerializeField]
        private bool _spawnPlayersRandomly = false;

        [Space]
        [SerializeField][Range(1, 50)]
        private int _enemyCount = 10;
        [SerializeField]
        private float _hexSize = 2.0f;
        [SerializeField][Range(1, 20)]
        private int _gridSize = 3;

        private Vector3 _playerOnePos;
        private Vector3 _playerTwoPos;

        private List<Vector3> HexGridData = new List<Vector3>();

        public void Start()
        {
            if (gameLoop == null)
                gameLoop = this;

            var board = new Board<Piece<Hex>, Hex>();
            var grid = new HexGrid<Hex>(_gridSize);

            DeleteHexField();
            DeletePieces();

            GenerateHexField(grid);
            GeneratePieces(grid);

            ConnectPiece(board, grid);
            var playerOne = GetPlayerPiece(board, grid, _playerOnePos);
            var playerTwo = GetPlayerPiece(board, grid, _playerTwoPos);

            //All states that the game uses are registered here
            _gameStateMachine = new StateMachine<GameStateBase>();
            _gameStateMachine.Register(GameStateBase.FirstPlayerGameState, 
                new FirstPlayerGameState(_gameStateMachine, board, grid, _gridSize, playerOne, _playerOneHand));
            _gameStateMachine.Register(GameStateBase.SecondPlayerGameState, 
                new SecondPlayerGameState(_gameStateMachine, board, grid, _gridSize, playerTwo, _playerTwoHand));

            //The starting state of the game
            _gameStateMachine.InitialState = GameStateBase.FirstPlayerGameState;

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
                piece.gameObject.SetActive(false);
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

            if (_spawnPlayersRandomly)
            {
                SpawnPlayer(grid, usedPositions, _playerOnePrefab, out var posOne);
                _playerOnePos = posOne;
                SpawnPlayer(grid, usedPositions, _playerTwoPrefab, out var posTwo);
                _playerTwoPos = posTwo;
            }
            else
            {
                int playerOnePos = grid.CubeCoordinates.IndexOf(new Vector3(0, 0, 0));

                Instantiate(_playerOnePrefab, Vector3.zero, Quaternion.identity);
                _playerOnePos = Vector3.zero;

                usedPositions.Add(playerOnePos);

                int playerTwoPos = grid.CubeCoordinates.IndexOf(new Vector3(1, -1, 0));

                Instantiate(_playerTwoPrefab, Vector3.zero, Quaternion.identity);
                _playerTwoPos = new Vector3(1, -1, 0);

                usedPositions.Add(playerTwoPos);
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

        private void SpawnPlayer(HexGrid<Hex> grid, List<int> usedPositions, GameObject playerPrefab, out Vector3 playerPosition)
        {
            var randomPlayerPos = Random.Range(0, grid.CubeCoordinates.Count);
            var playerPos = _coordinateConverter.CubeCoordinatesToCartesian
                    (grid.CubeCoordinates[randomPlayerPos], _hexSize);

            Instantiate(playerPrefab, playerPos, Quaternion.identity);
            playerPosition = playerPos;

            usedPositions.Add(randomPlayerPos);
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
                    //Debug.Log($"Piece placed on hex {(v, a, l)}");
                }
            }
        }

        private Piece<Hex> GetPlayerPiece(Board<Piece<Hex>, Hex> board, HexGrid<Hex> grid, Vector3 playerPos)
        {
            var playerCubePos = _coordinateConverter.CartesianCoordinatesToCube(playerPos, _hexSize);

            if (grid.TryGetPositionAt(playerCubePos.v, playerCubePos.a, playerCubePos.l, out Hex hex))
            {
                board.TryGetPiece(hex, out var piece);
                return piece;

                //Debug.Log($"Selected piece player ID: {piece.PlayerID} on worldposition {_playerPos} and cubecoordinate {playerCubePos}");
            }
            else
            {
                return null;
            }
        }

        private void CreateHex(Vector3 cartesianCoordinate, out Hex hex)
        {
            var hexObject = Instantiate(_hexPrefab, cartesianCoordinate, _hexPrefab.transform.rotation, transform);
            hex = hexObject.GetComponentInChildren<Hex>();
            //hex.Model = hex;
        }

        public void SelectValidPositions(CardType cardType)
            => _gameStateMachine.CurrentState.SelectValidPositions(cardType);
        public void DeselectValidPositions(CardType cardType)
            => _gameStateMachine.CurrentState.DeselectValidPositions(cardType);
        public void SelectIsolated(CardType cardType, Hex hex)
            => _gameStateMachine.CurrentState.SelectIsolated(cardType, hex);
        public void DeselectIsolated()
            => _gameStateMachine.CurrentState.DeselectIsolated();
        public void ExecuteCard(CardType cardType, Hex hex)
            => _gameStateMachine.CurrentState.ExecuteCard(cardType, hex);

        public void Forward()
            => _gameStateMachine.CurrentState.Forward();
    }
}