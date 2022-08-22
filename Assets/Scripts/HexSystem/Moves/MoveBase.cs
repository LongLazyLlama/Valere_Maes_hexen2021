using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoardSystem;
using CardSystem;
using UnityEngine;

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

        public void Execute(Piece<TPosition> piece, TPosition mouseHexPosition, 
            List<TPosition> isolatedPositions, List<TPosition> targetPositions, CardType cardType)
        {
            switch (cardType)
            {
                case CardType.Pushback :
                {
                    for (int i = 0; i < isolatedPositions.Count; i++)
                    {
                        if (targetPositions[i] != null)
                        {
                            var pieceWithinRange = Board.TryGetPiece(isolatedPositions[i], out var toPiece);
                            var pieceOnDestination = Board.TryGetPiece(targetPositions[i], out var p);

                            if (pieceWithinRange && !pieceOnDestination)
                                Board.Move(toPiece, targetPositions[i]);
                        }
                    }
                    break;
                }
                case CardType.Teleport :
                {
                    Board.Move(piece, mouseHexPosition);
                    break;
                }
                case CardType.Slash :
                {
                    TakeEnemiesOnIsolated(isolatedPositions);
                    break;
                }
                case CardType.Swipe :
                {
                    TakeEnemiesOnIsolated(isolatedPositions);
                    break;
                }
                case CardType.Rain :
                {
                    DestroyRandomHexes(3, targetPositions);
                    break;
                }
            }

            Debug.Log($"Card {cardType} was executed.");
        }

        private void DestroyRandomHexes(int Amount, List<TPosition> positions)
        {
            for (int i = 0; i < Amount; i++)
            {
                var randomNumber = UnityEngine.Random.Range(0, positions.Count);
                var randomPosition = positions[randomNumber];
                var pieceTaken = Board.TryGetPiece(randomPosition, out var toPiece);

                //If there is a piece on a hex, remove it.
                if (pieceTaken)
                {
                    if (toPiece.PlayerID == 1)
                    {
                        Debug.Log("Player taken.");
                    }
                    Board.Take(toPiece);
                }

                positions.RemoveAt(randomNumber);

                HexGrid.RemoveHex(randomPosition);
                randomPosition.DestroyHex();
            }
        }

        private void TakeEnemiesOnIsolated(List<TPosition> isolatedPositions)
        {
            foreach (var pos in isolatedPositions)
            {
                //If there is a piece on the new position, take it.
                if (pos != null)
                {
                    var pieceTaken = Board.TryGetPiece(pos, out var toPiece);
                    if (pieceTaken)
                        Board.Take(toPiece);
                }
            }
        }

        public abstract List<TPosition> Positions(Piece<TPosition> piece);
    }
}
