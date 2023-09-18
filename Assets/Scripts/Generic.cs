using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace MarchingCubesGeneric
{
    public struct Cube
    {
        public float[] weights;
        public Vector3[] vertices;
    }

    public struct Triangle
    {
        public Vector3 vtx0;
        public Vector3 vtx1;
        public Vector3 vtx2;
    }

    public static class Consts
    {
        public static readonly int pointsPerAxis = 21;
        public static readonly int cubesPerAxis = 20;

        public static int pointSize => pointsPerAxis * pointsPerAxis * pointsPerAxis;
        public static int cubeSize => cubesPerAxis * cubesPerAxis * cubesPerAxis;
    }
}
