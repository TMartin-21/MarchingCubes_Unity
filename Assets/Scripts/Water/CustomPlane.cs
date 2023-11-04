using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CustomPlane : MonoBehaviour
{
    [Header("Plane size")]
    public Vector2Int size;

    [Range(1, 400)]
    public int tesselationLevel = 1;

    private Mesh mesh;
    private List<Vector3> vertexData;
    private List<int> triangles;

    private void Start()
    {
        mesh = new Mesh();
        vertexData = new List<Vector3>();
        triangles = new List<int>();
        Create();
    }

    private Vector3[] _vertices;
    private int[] _triangles;

    private void Create()
    {
        vertexData.Clear();
        triangles.Clear();
      
        int M = size.x * tesselationLevel;
        int N = size.y * tesselationLevel;
        float stripSize = 1f / tesselationLevel;

        int nVertex = (M + 1) * (N + 1);
        _vertices = new Vector3[nVertex];
        for (int y = 0; y < N + 1; y++)
        {
            for (int x = 0; x < M + 1; x++)
            {
                _vertices[x + (M + 1) * y] = new Vector3(x * stripSize, 0, y * stripSize);
            }
        }

        int offset = 0;
        int index = 0;
        _triangles = new int[M * N * 6];
        int mVtxSize = M + 1;
        for (int j = 0; j < N; j++)
        {
            for (int i = 0; i < M; i++)
            {
                _triangles[index + 0] = offset;
                _triangles[index + 1] = offset + mVtxSize;
                _triangles[index + 2] = offset + mVtxSize + 1;

                _triangles[index + 3] = offset;
                _triangles[index + 4] = offset + mVtxSize + 1;
                _triangles[index + 5] = offset + 1;

                offset++;
                index += 6;
            }
            offset++;
        }

        CreateMesh();
    }

    private void CreateMesh()
    {
        mesh.Clear();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.name = "Custom Plane";
        mesh.vertices = _vertices;
        mesh.triangles = _triangles;
        mesh.Optimize();
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
}
