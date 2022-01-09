using System;
using System.Collections;
using System.Collections.Generic;
using HexSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameSystem
{
    //Saves the hex in eventargs.
    public class HexEventArgs : EventArgs
    {
        public Hex Hex;

        public HexEventArgs(Hex hex)
        {
            Hex = hex;
        }
    }

    public class Hex : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPosition
    {
        //public event EventHandler<HexEventArgs> Clicked;

        [SerializeField]
        private UnityEvent OnActivate;
        [SerializeField]
        private UnityEvent OnDeactivate;

        public bool Highlight
        {
            set
            {
                if (value)
                    OnActivate.Invoke();
                else
                    OnDeactivate.Invoke();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnDeactivate.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnActivate.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnDeactivate.Invoke();
        }

        //------------------------------------------------------------------------------------------------------

        //public void OnPointerClick(PointerEventData eventData)
        //{
        //    //Debug.Log($"Tile {gameObject.name} at position {_positionHelper.WorldToGridPosition()}")
        //    //FindObjectOfType<GameLoop>().DebugTile(this);
        //    OnClick(new HexEventArgs(this));
        //}

        //protected virtual void OnClick(HexEventArgs eventArgs)
        //{
        //    var handler = Clicked;
        //    handler?.Invoke(this, eventArgs);
        //}
    }
}
