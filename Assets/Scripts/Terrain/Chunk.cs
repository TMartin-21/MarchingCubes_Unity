using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingCubesGeneric;
using Unity.VisualScripting;

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
        TERRACED,
        GAUSSIAN
    }

    [Header("Terrain attributes")]
    public Terrain terrainType = Terrain.NORMAL;
    public float terraceHeight = 2f;
    public float gaussianAmplitude = 1;
    public float xSpread = 1;
    public float zSpread = 1;

    private Cube[] cubes;
    private Simplex3D simplexNoise;
    private MarchingCubes marchingCubes;

    private Vector3 initialChunkPos;

    private void Awake()
    {
        initialChunkPos = Vector3.up;

        cubes = new Cube[Consts.cubeSize];
        for (int x = 0; x < Consts.cubesPerAxis; x++)
            for (int y = 0; y < Consts.cubesPerAxis; y++)
                for (int z = 0; z < Consts.cubesPerAxis; z++)
                {
                    cubes[x + Consts.cubesPerAxis * (y + Consts.cubesPerAxis * z)].weights = new float[8];
                    cubes[x + Consts.cubesPerAxis * (y + Consts.cubesPerAxis * z)].vertices = new Vector3[8];
                }

        simplexNoise = GetComponent<Simplex3D>();
        marchingCubes = GetComponent<MarchingCubes>();
    }

    public void CreateMesh(Vector3 chunkPosition)
    {
        if (initialChunkPos == Vector3.up)
        {
            initialChunkPos = chunkPosition;
        }

        simplexNoise.NoiseShader(chunkPosition);
        SetCubeVertices(chunkPosition);
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
    
    private void SetCubeVertices(Vector3? chunkPosition = null)
    {
        if (chunkPosition == null)
        {
            chunkPosition = Vector3.zero;
        }

        for (int x = 0; x < Consts.cubesPerAxis; x++)
            for (int y = 0; y < Consts.cubesPerAxis; y++)
                for (int z = 0; z < Consts.cubesPerAxis; z++)
                {
                    int cubeIndex = x + Consts.cubesPerAxis * (y + Consts.cubesPerAxis * z);
                    float _x = ((float)chunkPosition?.x * Consts.cubesPerAxis + x);
                    float _y = ((float)chunkPosition?.y * Consts.cubesPerAxis + y);
                    float _z = ((float)chunkPosition?.z * Consts.cubesPerAxis + z);
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
        else if (terrainType == Terrain.GAUSSIAN)
        {
            return Gaussian(vertex, weightIndex);
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

    private float Gaussian(Vector3 cubePos, int weightIndex)
    {
        //TODO
        //Vector3 center = initialChunkPos + new Vector3(Consts.chunkCount / 2, 0, Consts.chunkCount / 2);
        Vector3 pos = cubePos - new Vector3(150, 0, 250);
        float value = - (pos.x * pos.x) / (2 * xSpread * xSpread) - (pos.z * pos.z) / (2 * zSpread * zSpread);
        float gaussian = gaussianAmplitude * Mathf.Exp(value);
        return -cubePos.y * 2f + gaussian + simplexNoise.Noise[weightIndex] - 110f;
    }
}
