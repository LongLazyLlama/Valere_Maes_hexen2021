using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CardSystem
{
    public class CardVisual : MonoBehaviour, IDragHandler, IDropHandler
    {
        [SerializeField]
        private CardType _cardType;
        [SerializeField]
        private Vector3 _offsetFromMouse = new Vector3(0, -100, 0);

        public CardType cardType => _cardType;

        private Vector3 _startPosition;

        private void Start()
        {
            _startPosition = this.transform.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            this.transform.position = Input.mousePosition + _offsetFromMouse;
            //this.gameObject.GetComponent<Image>().raycastTarget = false;
        }

        public void OnDrop(PointerEventData eventData)
        {
            this.transform.position = _startPosition;
            //this.gameObject.GetComponent<Image>().raycastTarget = true;
        }
    }

}