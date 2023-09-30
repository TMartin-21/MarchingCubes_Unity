using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CustomPlane : MonoBehaviour
{
    public ComputeShader computeVertexPos;

    [Header("Plane size")]
    public Vector2Int size;

    [Range(1, 100)]
    public int tesselationLevel = 1;

    struct VertexData
    {
        Vector3 position;
        Vector3 normal;
        Vector3 texcoord;
    }

    static Vector3[] offset =
    {
        new Vector3(0, 0, 0),
        new Vector3(0, 0, 1),
        new Vector3(1, 0, 0),
        new Vector3(1, 0, 1),
    };


    private Mesh mesh;

    List<Vector3> vertexData;
    List<int> triangles;

    private int index = 0;
    private ComputeBuffer vertexBuffer;

    private void Start()
    {
        mesh = new Mesh();
        vertexData = new List<Vector3>();
        triangles = new List<int>();
        index = 0;
    }

    private void Update()
    {
        Create();
    }

    private void Create()
    {
        vertexData.Clear();
        triangles.Clear();
      
        index = 0;
        int M = size.x * tesselationLevel;
        int N = size.y * tesselationLevel;
        float stripSize = 1f / tesselationLevel;

        Dictionary<Vector3, int> vertexHash = new Dictionary<Vector3, int>();
        vertexHash.Clear();

        Vector3[] vertices = new Vector3[6];
        
        for (int y = 0; y < N; y++)
        {
            for (int x = 0; x < M; x++)
            {
                vertices[0] = new Vector3(x * stripSize, 0, y * stripSize);
                vertices[1] = new Vector3(x * stripSize, 0, (y + 1) * stripSize);
                vertices[2] = new Vector3((x + 1) * stripSize, 0, (y + 1) * stripSize);
                
                
                vertices[3] = new Vector3(x * stripSize, 0, y * stripSize);
                vertices[4] = new Vector3((x + 1) * stripSize, 0, (y + 1) * stripSize);
                
                vertices[5] = new Vector3((x + 1) * stripSize, 0, y * stripSize);


                for (int i = 0; i < 6; i++)
                {
                    if (!vertexHash.ContainsKey(vertices[i]))
                    {
                        vertexHash.Add(vertices[i], index++);
                        vertexData.Add(vertices[i]);
                    }
                    triangles.Add(vertexHash[vertices[i]]);
                }
            }
        }

        CreateMesh();
    }

    private void CreateMesh()
    {
        mesh.Clear();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.name = "Custom Plane";
        mesh.vertices = vertexData.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    private void Shader()
    {
        int mStrip = size.x * tesselationLevel;
        int nStrip = size.y * tesselationLevel;
        float stripSize = 1.0f / tesselationLevel;
        int nVertex = (mStrip + 1) * (nStrip + 1);

        int kernel = computeVertexPos.FindKernel("WaterSurface");

        int workgroupX = Mathf.CeilToInt(mStrip / 8.0f);
        int workgroupY = Mathf.CeilToInt(nStrip / 8.0f);

        vertexBuffer = new ComputeBuffer(nVertex, sizeof(float) * 3);

        RenderTexture tex = new RenderTexture(mStrip + 1, nStrip + 1, 0,
                                              RenderTextureFormat.ARGBFloat,
                                              RenderTextureReadWrite.Linear);
        
        computeVertexPos.SetTexture(kernel, "Result", tex);
        
    }
}
