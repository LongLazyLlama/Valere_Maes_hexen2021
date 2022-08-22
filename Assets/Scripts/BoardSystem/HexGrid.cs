using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DAE.Commons;
using System;

namespace BoardSystem
{
    public class HexGrid<TPosition>
    {
        public int GridSize { get; }

        public BidirectionalDictionary<TPosition, (int v, int a, int l)> Positions
            = new BidirectionalDictionary<TPosition, (int v, int a, int l)>();

        public List<Vector3> CubeCoordinates = new List<Vector3>();

        //Creates a new hexgrid.
        public HexGrid(int range)
        {
            GridSize = range;
        }

        //Generates the cubeCoordinates for the grid.
        public List<Vector3> GenerateCubeCoordinates()
        {
            for (int v = -GridSize; v <= GridSize; v++)
            {
                for (int a = Mathf.Max(-GridSize, -v - GridSize); a <= Mathf.Min(GridSize, -v + GridSize); a++)
                {
                    var l = -v - a;

                    var cubeCoordinate = new Vector3(v, a, l);
                    CubeCoordinates.Add(cubeCoordinate);
                }
            }
            return CubeCoordinates;
        }

        //Takes a position or cubeCoordinate from the dictionary.
        public bool TryGetPositionAt(int v, int a, int l, out TPosition position)
            => Positions.TryGetKey((v, a, l), out position);

        public bool TryGetCubeCoordinateAt(TPosition position, out (int v, int a, int l) cubeCoordinate)
            => Positions.TryGetValue(position, out cubeCoordinate);

        //Adds all generated coordinates to the dictionary.
        public void Register(TPosition position, int v, int a, int l)
        {
            if (v < -GridSize || v > GridSize)
                throw new ArgumentException(nameof(v));

            if (a < -GridSize || a > GridSize)
                throw new ArgumentException(nameof(a));

            if (l < -GridSize || l > GridSize)
                throw new ArgumentException(nameof(l));

            Positions.Add(position, (v, a, l));

            //Debug.Log("Cube coordinate Registered at position: " + new Vector3(v, a, l));
        }

        public void RemoveHex(TPosition hex)
        {
            //Removes a random hex with its position from the dictionary.
            Positions.Remove(hex);
        }
    }
}