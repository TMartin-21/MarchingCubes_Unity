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
        public static readonly int pointsPerAxis = 51;
        public static readonly int cubesPerAxis = 50;
        public static readonly int chunkCount = 9;

        public static int pointSize => pointsPerAxis * pointsPerAxis * pointsPerAxis;
        public static int cubeSize => cubesPerAxis * cubesPerAxis * cubesPerAxis;
    }
}
