using System.Collections.Generic;
using UnityEngine;

namespace CanDurmazLibrary
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }
    public class ObjectPooler : MonoBehaviour
    {
        public static ObjectPooler Instance;
        public List<Pool> pools;
        public Dictionary<string, Queue<GameObject>> poolDictionary;
        public Transform poolParent;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            poolDictionary = new Dictionary<string, Queue<GameObject>>();
            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab, poolParent);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }

                poolDictionary.Add(pool.tag, objectPool);
            }
        }

        public GameObject GetPooledObject(string tag, Vector3 pos, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
                return null;
            }

            Queue<GameObject> objectPool = poolDictionary[tag];

            foreach (GameObject obj in objectPool)
            {
                if (!obj.activeInHierarchy)
                {
                    obj.SetActive(true);
                    obj.transform.position = pos;
                    obj.transform.rotation = rotation;
                    return obj;
                }
            }
            Pool pool = pools.Find(p => p.tag == tag);
            if (pool != null)
            {
                GameObject obj = Instantiate(pool.prefab, poolParent);
                obj.SetActive(true);
                obj.transform.position = pos;
                obj.transform.rotation = rotation;
                objectPool.Enqueue(obj);
                return obj;
            }

            Debug.LogWarning("No pool found with tag " + tag);
            return null;
        }
        public void ReturnObjectToPool(GameObject obj, string tag)
        {
            if (poolDictionary.ContainsKey(tag))
            {
                obj.SetActive(false);
            }
            else
            {
                Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            }
        }
    }
}