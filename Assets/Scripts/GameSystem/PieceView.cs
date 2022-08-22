using System;
using System.Collections;
using System.Collections.Generic;
using HexSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameSystem
{
    public class ClickEventArgs : EventArgs
    {
        public Piece<Hex> Piece { get; }

        public ClickEventArgs(Piece<Hex> piece)
        {
            Piece = piece;
        }
    }

    public class PieceView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private int _playerID;
        [SerializeField]
        private UnityEvent<bool> OnHighlight = null;

        public int PlayerID => _playerID;

        public bool Highlight
        {
            set
            {
                if (value)
                {
                    OnHighlight.Invoke(!value);
                }
            }
        }

        private Piece<Hex> model;
        public Piece<Hex> Model
        {
            get => model;
            set
            {
                if (model != null)
                {
                    model.Taken -= OnPieceTaken;
                    model.Placed -= OnPiecePlaced;
                    model.Moved -= OnPieceMoved;
                }

                model = value;

                if (model != null)
                {
                    model.Taken += OnPieceTaken;
                    model.Placed += OnPiecePlaced;
                    model.Moved += OnPieceMoved;
                }
            }
        }

        public event EventHandler<ClickEventArgs> Clicked;

        //Click stays for future piece selection/switching between pieces
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log($"clicked {gameObject.name}");

            OnClicked(new ClickEventArgs(Model));
        }

        protected virtual void OnClicked(ClickEventArgs e)
        {
            var handler = Clicked;
            handler?.Invoke(this, e);
        }

        //Moves the piece to a new position.
        private void OnPieceMoved(object sender, PieceEventArgs<Hex> e)
        {
            transform.position = e.Position.transform.position;
        }

        //Places the piece on a position.
        private void OnPiecePlaced(object sender, PieceEventArgs<Hex> e)
        {
            transform.position = e.Position.transform.position;
            gameObject.SetActive(true);
        }

        //Removes the piece from the board.
        private void OnPieceTaken(object sender, PieceEventArgs<Hex> e)
        {
            gameObject.SetActive(false);
        }

        //private void OnPieceActivationChanged(object source, PieceEventArgs eventArgs)
        //{
        //    Debug.Log("Activated " + eventArgs.Status);
        //}
    }
}

