using System;
using System.Collections;
using System.Collections.Generic;
using HexSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameSystem
{
    public class Hex : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPosition
    {
        [SerializeField]
        private UnityEvent OnActivate;
        [SerializeField]
        private UnityEvent OnDeactivate;

        private bool _isHighlighted;
        public bool Highlight
        {
            set
            {
                if (value)
                {
                    _isHighlighted = true;
                    OnActivate.Invoke();
                }
                else
                {
                    _isHighlighted = false;
                    OnDeactivate.Invoke();
                }
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (!_isHighlighted)
            {
                //Debug.Log("Dropped on hex.");
            }
            else
            {
                //Debug.Log("Dropped on highlighted hex.");

                GameObject droppedCard = eventData.pointerDrag;
                droppedCard.TryGetComponent<CardView>(out CardView cardView);

                GameLoop.gameLoop.DeselectIsolated();
                GameLoop.gameLoop.DeselectValidPositions(cardView.CardType);
                GameLoop.gameLoop.ExecuteCard(cardView.CardType, this);

                cardView.CardUsed = true;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            GameObject card = eventData.pointerDrag;
            GameLoop.gameLoop.DeselectIsolated();

            if (!_isHighlighted && card != null)
            {
                card.TryGetComponent<CardView>(out CardView cardView);

                GameLoop.gameLoop.SelectValidPositions(cardView.CardType);

                //Debug.Log("on hex");
            }
            if (_isHighlighted && card != null)
            {
                card.TryGetComponent<CardView>(out CardView cardView);

                GameLoop.gameLoop.DeselectValidPositions(cardView.CardType);

                GameLoop.gameLoop.SelectIsolated(cardView.CardType, this);

                //Debug.Log("on highlighted hex");
            }
        }
    }
}
