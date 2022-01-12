using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    [CreateAssetMenu(menuName = "HexField/CoordinateConverter")]
    public class CoordinateConverter : ScriptableObject
    {
        public Vector3 CubeCoordinatesToCartesian(Vector3 cubeCoordinate, float hexSize)
        {
            //Returns a cartesian coordinate representing the vector in worldspace.
            var hexWidth = Mathf.Sqrt(3) * hexSize;
            var HexHeight = hexSize * 2;

            var cartesianX = (cubeCoordinate.x * hexWidth) + (cubeCoordinate.z * (hexWidth / 2));
            var cartesianY = 0;
            var cartesianZ = cubeCoordinate.z * (HexHeight * (3.0f / 4.0f));

            return new Vector3(cartesianX, cartesianY, cartesianZ);
        }

        public (int v, int a, int l) CartesianCoordinatesToCube(Vector3 cartesianCoordinate, float hexSize)
        {
            //Returns a cube coordinate representing the vector in the hexgrid.
            var cubeX = Mathf.RoundToInt((Mathf.Sqrt(3) / 3 * cartesianCoordinate.x - 1.0f / 3 * cartesianCoordinate.z) / hexSize);
            var cubeZ = Mathf.RoundToInt((2.0f / 3 * cartesianCoordinate.z) / hexSize);
            var cubeY = Mathf.RoundToInt(-cubeX - cubeZ);

            //Debug.Log($"WorldPosition {cartesianCoordinate} converted to cube {new Vector3(cubeX, cubeY, cubeZ)}");

            return (cubeX, cubeY, cubeZ);
        }
    }
}
