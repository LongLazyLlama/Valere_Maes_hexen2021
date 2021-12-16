using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    [CreateAssetMenu(menuName = "HexField/CoordinateConverter")]
    public class CoordinateConverter : ScriptableObject
    {
        [SerializeField]
        public float _hexSize = 2.0f;

        public Vector3 CubeCoordinatesToCartesian(Vector3 cubeCoordinate)
        {
            //Returns a cartesian coordinate representing the vector in worldspace.
            var hexWidth = Mathf.Sqrt(3) * _hexSize;
            var HexHeight = _hexSize * 2;

            var cartesianX = (cubeCoordinate.x * hexWidth) + (cubeCoordinate.z * (hexWidth / 2)); 
            var cartesianY = 0;
            var cartesianZ = cubeCoordinate.z * (HexHeight * (3.0f / 4.0f));

            return new Vector3(cartesianX, cartesianY, cartesianZ);
        }

        public Vector3 CartesianCoordinatesToCube(Vector3 cartesianCoordinate)
        {
            //Returns a cube coordinate representing the vector in the hexgrid.

            var cubeX = (Mathf.Sqrt(3) / 3 * cartesianCoordinate.x - 1.0f / 3 * cartesianCoordinate.z) / _hexSize;
            var cubeZ = (2.0f / 3 * cartesianCoordinate.z) / _hexSize;
            var cubeY = -cubeX -cubeZ;

            //Debug.Log($"WorldPosition {cartesianCoordinate} converted to cube {new Vector3(cubeX, cubeY, cubeZ)}");

            return new Vector3(cubeX, cubeY, cubeZ);
        }

        //public HexData AddToCubeCoordinate(Vector3 vectorData)
        //{
        //    //Adds any vector data to the hex (positions, rotations, etc..)
        //    Vector3 cubeCoordinate = new Vector3(
        //        CubeCoordinates.x + vectorData.x,
        //        CubeCoordinates.y + vectorData.y,
        //        CubeCoordinates.z + vectorData.z);

        //    return new HexData(cubeCoordinate);
        //}

        //public HexData SubtractFromCubeCoordinate(Vector3 vectorData)
        //{
        //    Vector3 cubeCoordinate = new Vector3(
        //        CubeCoordinates.x - vectorData.x,
        //        CubeCoordinates.y - vectorData.y,
        //        CubeCoordinates.z - vectorData.z);

        //    return new HexData(cubeCoordinate);
        //}
    }
}
