using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [System.Serializable]
    public struct Pool
    {
        public string tag;
        public GameObject gameObject;
        public int size;
    }

    public static ObjectPool instance;

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;
    
    public GameObject objectToPool;
    
    public int amountToPool;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        
    }
}
