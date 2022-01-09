using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CardSystem;
using System;

namespace GameSystem
{
    public class CardView : MonoBehaviour, IDragHandler, IDropHandler
    {
        [SerializeField]
        private CardType _cardType;
        [SerializeField]
        private Vector3 _offsetFromMouse = new Vector3(0, -100, 0);

        private Vector3 _startPosition;

        private void Start()
        {
            _startPosition = this.transform.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            GameLoop.gameLoop.CardSelected(_cardType);
            this.transform.position = Input.mousePosition + _offsetFromMouse;
            //this.gameObject.GetComponent<Image>().raycastTarget = false;
        }

        public void OnDrop(PointerEventData eventData)
        {
            GameLoop.gameLoop.CardDeSelected(_cardType);
            this.transform.position = _startPosition;
            //this.gameObject.GetComponent<Image>().raycastTarget = true;
        }
    }
}