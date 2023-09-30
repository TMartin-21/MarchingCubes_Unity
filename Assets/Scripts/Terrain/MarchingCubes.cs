using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using MarchingCubesGeneric;

[RequireComponent(typeof(Simplex3D))]
public class MarchingCubes : MonoBehaviour
{
    [Header("Marching cubes comupte shader")]
    public ComputeShader shader;

    [Header("Mesh Surface")]
    public float isolevel = 0f;
    public bool blendedMesh = false;

    private ComputeBuffer triangleBuffer;
    private Triangle[] _triangles;
    private ComputeBuffer countBuffer;
    private ComputeBuffer weightBuffer;
    private List<Vector3> vertices;
    private List<int> triangulation;

    public Mesh Mesh
    {
        private set;
        get;
    }

    private Dictionary<int, Vector3> vertexTriangulation;

    private void Awake()
    {
        triangleBuffer = new ComputeBuffer(Consts.pointSize * 5, sizeof(float) * 3 * 3, ComputeBufferType.Append);
        triangleBuffer.SetCounterValue(0);
        weightBuffer = new ComputeBuffer(Consts.pointSize, sizeof(float));
        countBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.IndirectArguments);

        vertices = new List<Vector3>();
        triangulation = new List<int>();
        vertexTriangulation = new Dictionary<int, Vector3>();
    }

    private void OnDestroy()
    {
        triangleBuffer.Release();
        weightBuffer.Release();
        countBuffer.Release();
    }

    private int GetCubeIndex(Cube cube)
    {
        int index = 0;
        for (int i = 0; i < 8; i++)
            if (cube.weights[i] < isolevel)
                index |= 1 << i;
        return index;
    }

    private void Triangulation(Cube cube)
    {
        int cubeIndex = GetCubeIndex(cube);
        int[] tri = LookupTables.triangulation[cubeIndex];
        for (int i = 0; tri[i] != -1; i++)
        {
            int edge = tri[i];
            int a = LookupTables.edges[edge][0];
            int b = LookupTables.edges[edge][1];

            Vector3 vertexA = cube.vertices[a];
            Vector3 vertexB = cube.vertices[b];

            Vector3 vertex = vertexA + (isolevel - cube.weights[a]) * (vertexB - vertexA) / (cube.weights[b] - cube.weights[a]);
            vertices.Add(vertex);

            int triangleCount = triangulation.Count;
            triangulation.Add(triangleCount);
        }
    }

    public void MarchingcubesCPU(Cube[] cubes)
    {
        vertices.Clear();
        triangulation.Clear();
        vertexTriangulation.Clear();

        for (int x = 0; x < Consts.cubesPerAxis; x++)
            for (int y = 0; y < Consts.cubesPerAxis; y++)
                for (int z = 0; z < Consts.cubesPerAxis; z++)
                {
                    if (blendedMesh)
                    {
                        BlendedMesh(cubes[x + Consts.cubesPerAxis * (y + Consts.cubesPerAxis * z)]);
                    }
                    else
                    {
                        Triangulation(cubes[x + Consts.cubesPerAxis * (y + Consts.cubesPerAxis * z)]);
                    }
                }


        BuildMesh();
    }

    public void MarchingCubesGPU()
    {

    }

    void BlendedMesh(Cube cube)
    {
        int cubeIndex = GetCubeIndex(cube);
        int edgeMask = LookupTables.edgeMask[cubeIndex];
        List<int> edgeIndicies = new List<int>();
        for (int i = 0; i < 12; i++)
            if ((edgeMask & (1 << i)) == (1 << i))
                edgeIndicies.Add(i);
        
        for (int i = 0; i < edgeIndicies.Count; i++)
        {
            int a = LookupTables.edges[edgeIndicies[i]][0];
            int b = LookupTables.edges[edgeIndicies[i]][1];

            Vector3 vertexA = cube.vertices[a];
            Vector3 vertexB = cube.vertices[b];
            Vector3 vertex = vertexA + (isolevel - cube.weights[a]) * (vertexB - vertexA) / (cube.weights[b] - cube.weights[a]);

            if (!vertices.Contains(vertex))
            {
                vertices.Add(vertex);
            }

            if (!vertexTriangulation.ContainsKey(edgeIndicies[i]))
            {
                vertexTriangulation.Add(edgeIndicies[i], vertex);
            }
            else
            {
                vertexTriangulation[edgeIndicies[i]] = vertex;
            }
        }

        int[] tri = LookupTables.triangulation[cubeIndex];
        for (int i = 0; tri[i] != -1; i++)
        {
            triangulation.Add(vertices.IndexOf(vertexTriangulation[tri[i]]));
        }
    }

    private void BuildMesh()
    {
        Mesh = new Mesh();
        Mesh.Clear();
        Mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        Mesh.name = "Marching Cubes";
        Mesh.vertices = vertices.ToArray();
        Mesh.triangles = triangulation.ToArray();
        Mesh.Optimize();
        Mesh.RecalculateNormals();
    }

    
    public void MarchingShader(float[] weights)
    {
        vertices.Clear();
        triangulation.Clear();
        triangleBuffer.SetCounterValue(0);
     
        int kernel = shader.FindKernel("MarchingCubes");

        //shader.SetInt("size", Consts.cubesPerAxis);
        shader.SetFloat("isolevel", isolevel);

        weightBuffer.SetData(weights);
        shader.SetBuffer(kernel, "weightBuffer", weightBuffer);
        shader.SetBuffer(kernel, "triangleBuffer", triangleBuffer);

        int workgroup = Mathf.CeilToInt(Consts.cubesPerAxis / 8f);
        shader.Dispatch(kernel, workgroup, workgroup, workgroup);

        
        ComputeBuffer.CopyCount(triangleBuffer, countBuffer, 0);
        int[] counter = new int[1] { 0 };
        countBuffer.GetData(counter);
        int count = counter[0];

        _triangles = new Triangle[count];
        triangleBuffer.GetData(_triangles);
        for (int i = 0; i < count; i++)
        {
            vertices.Add(_triangles[i].vtx0);
            int c = triangulation.Count;
            triangulation.Add(c);

            vertices.Add(_triangles[i].vtx1);
            int c1 = triangulation.Count;
            triangulation.Add(c1);

            vertices.Add(_triangles[i].vtx2);
            int c2 = triangulation.Count;
            triangulation.Add(c2);
        }

        BuildMesh();
    }
}
