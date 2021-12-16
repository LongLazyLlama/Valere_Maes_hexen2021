using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexField;

namespace GameSystem
{
    public class GameLoop : MonoBehaviour
    {
        [SerializeField]
        private CoordinateConverter _coordinateConverter;

        public Field HexField;

        public void Start()
        {
            HexField.Converter = _coordinateConverter;
        }
    }
}