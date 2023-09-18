using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingCubesGeneric;

[RequireComponent(typeof(Simplex3D))]
[RequireComponent(typeof(MarchingCubes))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Chunk : MonoBehaviour
{
    public enum Terrain
    {
        NORMAL,
        FLAT,
        TERRACED
    }

    [Header("Terrain attributes")]
    public Terrain terrainType = Terrain.NORMAL;
    public float terraceHeight = 2f;

    private Cube[] cubes;
    private Simplex3D simplexNoise;
    private MarchingCubes marchingCubes;

    private void Awake()
    {
        cubes = new Cube[Consts.cubeSize];
        for (int x = 0; x < Consts.cubesPerAxis; x++)
            for (int y = 0; y < Consts.cubesPerAxis; y++)
                for (int z = 0; z < Consts.cubesPerAxis; z++)
                {
                    cubes[x + Consts.cubesPerAxis * (y + Consts.cubesPerAxis * z)].weights = new float[8];
                    cubes[x + Consts.cubesPerAxis * (y + Consts.cubesPerAxis * z)].vertices = new Vector3[8];
                }
    }

    void Start()
    {
        simplexNoise = GetComponent<Simplex3D>();
        marchingCubes = GetComponent<MarchingCubes>();
    }

    // Update is called once per frame
    void Update()
    {
        //simplexNoise.GenerateNoise();
        SetCubeVertices();
        SetCubeValues();
        
        marchingCubes.MarchingcubesCPU(cubes);
        //marchingCubes.MarchingShader(simplexNoise.Noise);
        GetComponent<MeshFilter>().sharedMesh = marchingCubes.Mesh;
    }

    // For reloading chunks
    public void CreateMesh(/*Vector3 chunkPosition*/)
    {
        simplexNoise.NoiseShader();
        SetCubeVertices();
        SetCubeValues();
        marchingCubes.MarchingcubesCPU(cubes);
        GetComponent<MeshFilter>().sharedMesh = marchingCubes.Mesh;
    }

    private void SetCubeValues()
    {
        for (int x = 0; x < Consts.cubesPerAxis; x++)
            for (int y = 0; y < Consts.cubesPerAxis; y++)
                for (int z = 0; z < Consts.cubesPerAxis; z++)
                {
                    int cubeIndex = x + Consts.cubesPerAxis * (y + Consts.cubesPerAxis * z);
                    for (int i = 0; i < 8; i++)
                    {
                        Vector3 v = LookupTables.vertices[i];
                        Vector3Int w = new Vector3Int(x, y, z) + new Vector3Int((int)v.x, (int)v.y, (int)v.z);
                        int weightIndex = w.x + Consts.pointsPerAxis * (w.y + Consts.pointsPerAxis * w.z);
                        cubes[cubeIndex].weights[i] =  Density(cubes[cubeIndex].vertices[i], weightIndex);
                    }
                }
    }
    
    private void SetCubeVertices(/*Vector3 chunkPosition*/)
    {
        for (int x = 0; x < Consts.cubesPerAxis; x++)
            for (int y = 0; y < Consts.cubesPerAxis; y++)
                for (int z = 0; z < Consts.cubesPerAxis; z++)
                {
                    int cubeIndex = x + Consts.cubesPerAxis * (y + Consts.cubesPerAxis * z);
                    float _x = /*(chunkPosition.x * cubesPerAxis +*/ x;//);
                    float _y = /*(chunkPosition.y * cubesPerAxis +*/ y;//);
                    float _z = /*(chunkPosition.z * cubesPerAxis +*/ z;//);
                    for (int i = 0; i < 8; i++)
                        cubes[cubeIndex].vertices[i] = new Vector3(_x, _y, _z) + LookupTables.vertices[i];
                }
    }

    private float Density(Vector3 vertex, int weightIndex)
    {
        if (terrainType == Terrain.FLAT)
        {
            return FlatTerrain(vertex.y, weightIndex);
        }
        else if (terrainType == Terrain.TERRACED)
        {
            return Terracing(vertex.y, weightIndex);
        }
        return simplexNoise.Noise[weightIndex];
    }

    private float FlatTerrain(float height, int weightIndex)
    {
        return -height + simplexNoise.Noise[weightIndex];
    }

    private float Terracing(float height, int weightIndex)
    {
        return -height + simplexNoise.Noise[weightIndex] + height % terraceHeight;
    }
}
