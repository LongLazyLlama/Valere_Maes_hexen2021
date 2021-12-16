using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HexField
{
    public class HexVisual : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private UnityEvent OnActivate;
        [SerializeField]
        private UnityEvent OnDeactivate;

        public Vector3 CubeCoordinate;
        public Vector3 CartesianCoordinate;

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

            //Debug.Log($"Tile clicked at WP {CartesianCoordinate} and CC {CubeCoordinate}");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnActivate.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnDeactivate.Invoke();
        }
    }
}
