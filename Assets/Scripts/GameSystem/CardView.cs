using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CardSystem;
using System;

namespace GameSystem
{
    public class CardView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public CardType CardType;

        [SerializeField]
        private Vector3 _offsetFromMouse = new Vector3(0, -100, 0);

        [HideInInspector]
        public bool CardUsed;

        private CanvasGroup _canvasGroup;
        private Vector3 _startPosition;

        private void Start()
        {
            _startPosition = this.transform.position;
            _canvasGroup = this.GetComponentInParent<CanvasGroup>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = false;
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            this.transform.position = Input.mousePosition + _offsetFromMouse;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            GameLoop.gameLoop.DeselectValidPositions(CardType);
            _canvasGroup.blocksRaycasts = true;

            if (CardUsed)
            {
                DeckManager.Deck.CardUsed(_startPosition);
                Destroy(this.gameObject);
            }
            else
                this.transform.position = _startPosition;
        }
    }
}