using System;
using System.Collections;
using System.Collections.Generic;
using DAE.Commons;
using UnityEngine;

namespace BoardSystem
{
    //Eventargs stores event data, in this case when a piece does something, it saves its position and piece
    public class PiecePlacedEventArgs<TPiece, TPosition> : EventArgs
    {
        public TPosition AtPosition { get; }
        public TPiece Piece { get; }

        public PiecePlacedEventArgs(TPiece piece, TPosition position)
        {
            Piece = piece;
            AtPosition = position;
        }
    }
    public class PieceMovedEventArgs<TPiece, TPosition> : EventArgs
    {
        public TPiece Piece { get; }
        public TPosition FromPosition { get; }
        public TPosition ToPosition { get; }

        public PieceMovedEventArgs(TPiece piece, TPosition fromPosition, TPosition toPosition)
        {
            Piece = piece;
            FromPosition = fromPosition;
            ToPosition = toPosition;
        }
    }
    public class PieceRemoveEventArgs<TPiece, TPosition> : EventArgs
    {
        public TPosition FromPosition { get; }
        public TPiece Piece { get; }

        public PieceRemoveEventArgs(TPiece piece, TPosition position)
        {
            Piece = piece;
            FromPosition = position;
        }
    }

    public class Board<TPiece, TPosition>
    {
        //zonder event kan deze waarde reset of overschreven worden.
        public event EventHandler<PiecePlacedEventArgs<TPiece, TPosition>> PiecePlaced;
        public event EventHandler<PieceMovedEventArgs<TPiece, TPosition>> PieceMoved;
        public event EventHandler<PieceRemoveEventArgs<TPiece, TPosition>> PieceTaken;

        //Stores all units on the board with their representative position.
        private BidirectionalDictionary<TPiece, TPosition> _pieces
            = new BidirectionalDictionary<TPiece, TPosition>();

        //The 'key' in the dictionary refers to the position saved, and the 'value' to the piece.
        public bool TryGetPiece(TPosition position, out TPiece piece)
            => _pieces.TryGetKey(position, out piece);

        public bool TryGetPosition(TPiece piece, out TPosition position)
            => _pieces.TryGetValue(piece, out position);

        //Place 'places' pieces on the board => checks if it already exists, and if not, adds it to the dictionary.
        public void Place(TPiece piece, TPosition position)
        {
            //if the piece already exists, return.
            if (_pieces.ContainsKey(piece))
                return;

            //if the position already exists, return.
            if (_pieces.ContainsValue(position))
                return;

            //Adds the piece with its position to the dictionary.
            _pieces.Add(piece, position);

            //Debug.Log($"Pieces registered in dictionary: {_pieces.Count}");

            //saves the placement in eventargs for the replaymanager.
            OnPiecePlaced(new PiecePlacedEventArgs<TPiece, TPosition>(piece, position));
        }

        public void Move(TPiece piece, TPosition toPosition)
        {
            //Tries to get the position of the piece and stores it in fromposition, otherwise return.
            if (!TryGetPosition(piece, out var fromPosition))
                return;

            //If it fails to get the piece, return. OUT returns nothing because you already have the piece variable.
            if (TryGetPiece(toPosition, out _))
                return;

            //Removes the piece from the list so it can be added again with the new position, if it fails, return.
            if (!_pieces.Remove(piece))
                return;

            //Adds the piece with its position to the dictionary.
            _pieces.Add(piece, toPosition);

            //saves the move in eventargs for the replaymanager.
            OnPieceMoved(new PieceMovedEventArgs<TPiece, TPosition>(piece, fromPosition, toPosition));
        }

        public void Take(TPiece piece)
        {
            //If the position of the piece on the board is not found, return for it no longer exists.
            if (!TryGetPosition(piece, out var fromPosition))
                return;

            //If the piece is succesfully removed, saves its removal to the replaymanager's eventargs.
            if (_pieces.Remove(piece))
                //saves the removal in eventargs for the replaymanager.
                OnPieceTaken(new PieceRemoveEventArgs<TPiece, TPosition>(piece, fromPosition));
        }

        //These work for the replaymanager. They keep track of moves and placement/removal of pieces on the board.
        protected virtual void OnPiecePlaced(PiecePlacedEventArgs<TPiece, TPosition> eventArgs)
        {
            var handler = PiecePlaced; //een copy voor moest het aangepast worden.

            //'this' is the sender aka the board object and in this case eventargs invokes the 'placed' event.
            handler?.Invoke(this, eventArgs);
        }
        protected virtual void OnPieceMoved(PieceMovedEventArgs<TPiece, TPosition> eventArgs)
        {
            var handler = PieceMoved;
            handler?.Invoke(this, eventArgs);
        }
        protected virtual void OnPieceTaken(PieceRemoveEventArgs<TPiece, TPosition> eventArgs)
        {
            var handler = PieceTaken;
            handler?.Invoke(this, eventArgs);
        }
    }
}