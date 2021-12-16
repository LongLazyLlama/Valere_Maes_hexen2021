using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexField;

namespace GameSystem
{
    public class Field : MonoBehaviour
    {

        [SerializeField]
        private GameObject _hexPrefab;
        [SerializeField][Range(1, 20)]
        private int _gridSize = 3;

        private int currentGridSize;

        internal CoordinateConverter Converter;

        private List<Vector3> HexGridData = new List<Vector3>();

        private void Update()
        {
            //Creates the option to adjust the hexfield's size at runtime.
            if (currentGridSize != _gridSize)
            {
                DeleteHexField();
                GenerateHexField();

                currentGridSize = _gridSize;
            }
        }

        private void DeleteHexField()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        public void GenerateHexField()
        {
            //for gridsize create a cubecoordinate for each hex.
            //put them in a list
            HexGridData = GenerateCubeCoordinates();

            //for each cubecoodinate in the list => convert to cartesian coordinates
            foreach (Vector3 cubeCoordinate in HexGridData)
            {
                var cartesianCoordinate = Converter.CubeCoordinatesToCartesian(cubeCoordinate);
                cartesianCoordinate += this.transform.position;

                //instantiate a hex on every cartesian coordinate.
                CreateHex(cubeCoordinate , cartesianCoordinate);
            }
        }

        private List<Vector3> GenerateCubeCoordinates()
        {
            List<Vector3> cubeCoordinates = new List<Vector3>();

            for (int v = -_gridSize; v <= _gridSize; v++)
            {
                for (int a = Mathf.Max(-_gridSize, -v -_gridSize); a <= Mathf.Min(_gridSize, -v + _gridSize); a++)
                {
                    var l = -v - a;

                    var cubeCoordinate = new Vector3(v, a, l);
                    cubeCoordinates.Add(cubeCoordinate);

                    //Debug.Log("Cube coordinate created at position: " + new Vector3(v, a, l));
                }
            }
            return cubeCoordinates;
        }

        private void CreateHex(Vector3 cubeCoordinate, Vector3 cartesianCoordinate)
        {
            GameObject hex = Instantiate(_hexPrefab, cartesianCoordinate, _hexPrefab.transform.rotation, transform);

            hex.GetComponentInChildren<HexVisual>().CubeCoordinate = cubeCoordinate;
            hex.GetComponentInChildren<HexVisual>().CartesianCoordinate = cartesianCoordinate;
        }
    }
}