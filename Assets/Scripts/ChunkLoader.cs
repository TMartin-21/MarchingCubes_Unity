using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingCubesGeneric;

public class ChunkLoader : MonoBehaviour
{
    public event Action<int> rebuildMesh;

    public GameObject player;

    public Chunk[] chunks;

    private Vector3 lastPlayerPos;
    private Vector3 currentPlayerPos;


    private void Awake()
    {
        lastPlayerPos = Vector3.zero;
        currentPlayerPos = Vector3.zero;
    }

    private void Update()
    {
        UpdatePlayerPos();
        UpdateChunkPos();
    }

    private void UpdatePlayerPos()
    {
        Vector3 playerPos = player.transform.position;
        lastPlayerPos = currentPlayerPos;
        currentPlayerPos = new Vector3(playerPos.x / Consts.pointsPerAxis, 
                                       0, 
                                       playerPos.z / Consts.pointsPerAxis);
    }

    private void UpdateChunkPos()
    {

    }
}
