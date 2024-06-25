// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour
{
    #region Singleton

    public static ObjectPooler Instance;

    #endregion

    [System.Serializable]
    public class Pool
    {
        public ObjectsToSpawn tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;

    public Dictionary<ObjectsToSpawn, Queue<GameObject>> poolDictionary;

    public enum ObjectsToSpawn
    {
        coffee,
        chicken,
        hamburger,
        soup
    }

    private void Awake()
    {
        Instance = this;

        poolDictionary = new Dictionary<ObjectsToSpawn, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for(int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(ObjectsToSpawn objectToSpawnName, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(objectToSpawnName)) { Debug.LogWarning("Pool with tag " + objectToSpawnName + " not exist"); return null; }

        GameObject objectToSpawn = poolDictionary[objectToSpawnName].Dequeue();
        
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        poolDictionary[objectToSpawnName].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}