using System;
using System.Collections.Generic;
using MarchingCubesGeneric;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ChunkLoader : MonoBehaviour
{
    /* public Transform player;

     struct PlayerPos
     {
         public Vector3 last;
         public Vector3 current;
     }

     private PlayerPos playerPos;
     private int chunkCount = 0;
     private Chunk[] chunks;

     private int pointsPerChunk;

     private void Awake()
     {
         chunks = GetComponentsInChildren<Chunk>();
         chunkCount = chunks.Length;
         playerPos.last = player.position;
         playerPos.current = player.position;
     }

     private void LoadChunk(int id, Vector3 chunkPosition)
     {
         for (int i = 0; i < chunkCount; i++)
         {
             if (chunks[i].ID == id)
             {
                 chunks[i].LoadMesh(chunkPosition);
             }
         }
     }

     private void UpdatePlayerPosition()
     {
         playerPos.last = playerPos.current;
         playerPos.current = new Vector3(
                                 Mathf.Floor(player.position.x / pointsPerChunk),
                                 Mathf.Floor(player.position.y / pointsPerChunk),
                                 Mathf.Floor(player.position.z / pointsPerChunk)
                             );
     }

     private void GetChunkPositions()
     {
         List<Vector3> circle = new List<Vector3>();
         float n = 200;
         for (int i = 0; i < n; i++)
         {
             float fi = i * Mathf.PI / n;
             Vector3 pos = new Vector3(Mathf.Cos(fi), 0, Mathf.Sin(fi));
             circle.Add(pos);
         }
     }

     private void GetGrid()
     {
         for (int i = 0; i < 360; i++)
         {

         }
     }*/


    //??
    //public event Action rebuildMesh;

    public event Action<int> rebuildMesh;

    public GameObject player;

    [Header("Terrain attributes")]
    [Range(1, 8)]
    public int octave = 2;
    public float persitence = 0f;
    public float lacunarity = 0f;
    public int seed = 0;

    [Header("Chunk attributes")]
    public int cubesPerAxis = 20;

    public int chunkCount = 9;

    public Chunk[] chunks;

    private MeshFilter[] meshFilters;
    private CombineInstance[] combine;

    private List<Mesh> meshes;
    private bool built = false;

    private Vector3 lastPlayerPos;
    private Vector3 currentPlayerPos;
    private List<Vector3> lastChunkPos;
    private List<Vector3> currentChunkPos;

    int pointsPerChunk = 21;

    enum Type
    {
        UNCHANGED,
        CHANGED
    }

    private void Awake()
    {
        meshes = new List<Mesh>();

        lastPlayerPos = Vector3.zero;
        currentPlayerPos = Vector3.zero;
        lastChunkPos = new List<Vector3>(9);
        currentChunkPos = new List<Vector3>(9);
    }

    private void Start()
    {
        meshFilters = GetComponentsInChildren<MeshFilter>();
        combine = new CombineInstance[meshFilters.Length];
        chunks = GetComponentsInChildren<Chunk>();
        chunkCount = chunks.Length;

        Vector3[] chunkPositions = GetCurrentChunkPosition();
        for (int i = 0; i < chunkCount; i++)
        {
            chunks[i].CreateMesh(chunkPositions[i]);
        }

        CombineMeshes();
    }

    private void Update()
    {
        //LastAndCurrentPlayerPos();
        //LastAndCurrentChunkPositions();
        //LoadChunks();
        //CombineMeshes();
    }

    private void CombineMeshes()
    {
        meshFilters = GetComponentsInChildren<MeshFilter>();
        combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].mesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
            i++;
        }

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.CombineMeshes(combine);
        GetComponent<MeshFilter>().mesh = mesh;
        transform.gameObject.SetActive(true);
    }

    private void LoadChunks()
    {
        Vector3[] chunkPositions = GetCurrentChunkPosition();
        for (int i = 0; i < chunkCount; i++)
        {
                chunks[i].CreateMesh(chunkPositions[i]);
        }
    }

    private void LoadChunk()
    {
        if (ChangedPlayerPosition())
        {
            List<Vector3> changedPos = GetChunkPositions(Type.CHANGED);
            if (changedPos.Count == 0)
                return;

            List<Vector3> unChanged = GetChunkPositions(Type.UNCHANGED);
            List<Vector3> temp = lastChunkPos;
            for (int i = 0; i < unChanged.Count; i++)
                temp.Remove(unChanged[i]);

            for (int j = 0; j < changedPos.Count; j++)
            {

            }
        }
    }


    private void LastAndCurrentPlayerPos()
    {
        Vector3 playerPosition = player.transform.position;
        lastPlayerPos = currentPlayerPos;
        currentPlayerPos = new Vector3(
                            Mathf.Floor(playerPosition.x / pointsPerChunk),
                            0,
                            Mathf.Floor(playerPosition.z / pointsPerChunk));
    }

    private void LastAndCurrentChunkPositions()
    {
        for (int i = 0; i < 9; i++)
            lastChunkPos[i] = currentChunkPos[i];

        Vector3[] cPos = GetCurrentChunkPosition();
        for (int j = 0; j < 9; j++)
            currentChunkPos[j] = cPos[j];
    }

    private bool ChangedPlayerPosition()
    {
        if (!lastPlayerPos.Equals(currentPlayerPos))
            return true;
        return false;
    }

    private List<Vector3> GetChunkPositions(Type type)
    {
        List<Vector3> pos = new List<Vector3>();

        if (type == Type.CHANGED)
        {
            for (int i = 0; i < 9; i++)
                if (!lastChunkPos[i].Equals(currentChunkPos[i]))
                    pos.Add(currentChunkPos[i]);
        }
        else if (type == Type.UNCHANGED)
        {
            for (int i = 0; i < 9; i++)
                if (lastChunkPos[i].Equals(currentChunkPos[i]))
                    pos.Add(currentChunkPos[i]);
        }

        return pos;
    }

    private void DiscardChunks()
    {
        // Clear invalid meshes


    }

    //private void BuildMesh()
    //{
    //    Mesh mesh = new Mesh();
    //    mesh.CombineMeshes(combine);
    //    transform.GetComponent<MeshFilter>().sharedMesh = mesh;
    //    transform.gameObject.SetActive(true);
    //}

    static Vector3[] offset = new Vector3[] {  
      //new Vector3( 0,  0,  0),
      //new Vector3( 0,  0,  1),
      //new Vector3( 1,  0,  1),  
      //new Vector3( 1,  0,  0),  
      //new Vector3( 1,  0, -1),
      //new Vector3( 0,  0, -1),
      //new Vector3(-1,  0, -1),  
      //new Vector3(-1,  0,  0),  
      //new Vector3(-1,  0,  1),  

        new Vector3(0, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(1, 0, 1),
        new Vector3(0, 0, 1),
        new Vector3(-1, 0, 1),
        new Vector3(-1, 0, 0),
        new Vector3(1, 0, 2),
        new Vector3(0, 0, 2),
        new Vector3(-1, 0, 2),
    };

    private Vector3[] GetCurrentChunkPosition()
    {
        // Check the radial distance horizontally from the player
        // Determine wich unit chunk the player are
        // Calculate all the sourrunding chunks position (counteed from the origo)
        // Caluclate the noise and build mesh based on the chunk position (only once)
        // Load the chunks (only the meshes from the chunk, the chunk count will be fix,
        // only the chunk position will change, based on the player position)

        //List<Vector3> chunkPositions = new List<Vector3>(8);
        Vector3[] chunkPositions = new Vector3[chunkCount];
        Vector3 playerPosition = player.transform.position;
        chunkPositions[0] = new Vector3(Mathf.Floor(playerPosition.x / Consts.pointsPerAxis),
                                        0,
                                        Mathf.Floor(playerPosition.z / Consts.pointsPerAxis));

        for (int i = 1; i < chunkCount; i++)
            chunkPositions[i] = chunkPositions[0] + offset[i];

        return chunkPositions;
    }
}
