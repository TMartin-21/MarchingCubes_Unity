using System.Collections;
using System.Collections.Generic;
using System.Data;
using MarchingCubesGeneric;
using UnityEngine;

public class Simplex3D : MonoBehaviour
{
    [Header("Noise compute shader")]
    public ComputeShader shader;

    [Header("Noise attributes")]
    [Range(1, 8)]
    public int octave = 4;
    public float peristence = 0.75f;
    public float lacunarity = 2f;
    public int seed = 0;

    public float[] Noise
    {
        private set;
        get;
    }

    private ComputeBuffer noiseBuffer;

    private void Awake()
    {
        noiseBuffer = new ComputeBuffer(Consts.pointSize, sizeof(float));
        Noise = new float[Consts.pointSize];
    }

    private void OnDestroy()
    {
        noiseBuffer.Release();
    }

    private void Update()
    {
        NoiseShader();
    }

    public void NoiseShader(Vector3? chunkPosition = null)
    {
        if (chunkPosition == null)
        {
            chunkPosition = Vector3.zero;
        }
        int kernel = shader.FindKernel("CSMain");
        shader.SetBuffer(kernel, "noiseBuffer", noiseBuffer);
        shader.SetInt("width", Consts.pointsPerAxis);
        shader.SetInt("height", Consts.pointsPerAxis);
        shader.SetInt("depth", Consts.pointsPerAxis);
        shader.SetInt("octave", octave);
        shader.SetFloat("persistence", peristence);
        shader.SetFloat("lacunarity", lacunarity);

        // Noise need to be calculated based on the chunk position
        shader.SetInts("chunkPosition", (int)chunkPosition?.x, (int)chunkPosition?.y, (int)chunkPosition?.z);

        int workgroupX = Mathf.CeilToInt(Consts.pointsPerAxis / 8f);
        int workgroupY = Mathf.CeilToInt(Consts.pointsPerAxis / 8f);
        int workgroupZ = Mathf.CeilToInt(Consts.pointsPerAxis / 8f);
        shader.Dispatch(kernel, workgroupX, workgroupY, workgroupZ);

        noiseBuffer.GetData(Noise);
        //ReMapWeights();
    }

    private void ReMapWeights()
    {
        for (int i = 0; i < Noise.Length; i++)
        {
            Noise[i] = map(Noise[i], -1, 1, -8, 4);
        }
    }

    private float map(float value, float fromA, float fromB, float toA, float toB)
    {
        float perc = (value - fromA) / (fromB - fromA);
        return perc * (toB - toA) + toA;
    }
}
