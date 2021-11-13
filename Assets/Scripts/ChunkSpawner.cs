using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

[Serializable]
public class Chunk : MonoBehaviour
{
    public GameObject chunkPrefab;
    public Vector3 scenePrefabPosition;
    public bool isMemoryClearChunk;
}

public class ChunkSpawner : MonoBehaviour
{
    public List<AssetBundle> assetBundleList;
    //public List<Chunk> chunkOrder = new List<Chunk>();
    public List<GameObject> scenePrefabs = new List<GameObject>();
    public List<GameObject> spawnedPrefabs = new List<GameObject>();

    [Space]
    public List<Vector3> spawnPostions = new List<Vector3>();
    [Space]
    public int chunkIndex = 0;
    [Space]
    public GameObject lastAssetloaded;
    [Space]
    public GameObject loadedPrefabScene;
    [Space]
    public bool preLoadChunk = false;
    public bool overideSetSpawnPoints = true;

    void Start()
    {
        //AllocateChunksToMemory();
        //SetSpawnPositions();
    }

    // ---------------------------------------- Core Asset Logic ---------------------------- //

    public void AllocateChunksToMemory()
    {
        if (preLoadChunk)
        {
            LoadChunksFromAssets();
        }
    }


    public void LoadChunksFromAssets()
    {
        for (int i = 0; i < assetBundleList.Count; i++)
        {
            var scenePrefab = assetBundleList[i].LoadAllAssets().GetValue(0) as GameObject;

            if (scenePrefab)
            {
                scenePrefabs.Add(scenePrefab);
            }
            else
            {
                Debug.Log("LoadChunksFromAssets GameObject IS null!");
            }
        }
    }


    public void SetSpawnPositions()
    {
        if (preLoadChunk)
        {
            for (int i = 0; i < assetBundleList.Count; i++)
            {
                if (scenePrefabs[i].GetComponent<ChunkData>().scenePrefabPosition == Vector3.zero)
                {
                    if (overideSetSpawnPoints)
                    {
                        scenePrefabs[i].GetComponent<ChunkData>().scenePrefabPosition = spawnPostions[i];   
                    }
                    else
                    {
                        scenePrefabs[i].GetComponent<ChunkData>().scenePrefabPosition = 
                        new Vector3(scenePrefabs[spawnPostions.Count - 1].GetComponent<ChunkData>().scenePrefabPosition.x + 100f, 0f, 0f);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < scenePrefabs.Count; i++)
            {
                if (spawnedPrefabs[i].GetComponent<ChunkData>().scenePrefabPosition == Vector3.zero)
                {
                    if (overideSetSpawnPoints)
                    {
                        spawnedPrefabs[i].GetComponent<ChunkData>().scenePrefabPosition = spawnPostions[i];
                    }
                    else
                    {
                        spawnedPrefabs[i].GetComponent<ChunkData>().scenePrefabPosition =
                        new Vector3(spawnedPrefabs[spawnPostions.Count - 1].GetComponent<ChunkData>().scenePrefabPosition.x + 100f, 0f, 0f);
                    }
                }
            }
        }
    }


    public void SpawnNextChunk()
    {
        if (chunkIndex < assetBundleList.Count)
        {
            if (preLoadChunk)
            {
                loadedPrefabScene = Instantiate(scenePrefabs[chunkIndex].gameObject, scenePrefabs[chunkIndex].GetComponent<ChunkData>().scenePrefabPosition, scenePrefabs[chunkIndex].transform.rotation);
                spawnedPrefabs.Add(loadedPrefabScene);
                chunkIndex++;
            }
            else
            {
                loadedPrefabScene = assetBundleList[chunkIndex].LoadAllAssets().GetValue(0) as GameObject;

                Debug.Log(loadedPrefabScene.name + " is being loaded from assets.");

                if (loadedPrefabScene)
                {
                    Debug.Log(loadedPrefabScene.name + " Is loaded to memory.");

                    loadedPrefabScene = Instantiate
                        (
                        loadedPrefabScene,
                        loadedPrefabScene.GetComponent<ChunkData>().scenePrefabPosition,
                        loadedPrefabScene.transform.rotation
                        );

                    spawnedPrefabs.Add(loadedPrefabScene);
                    chunkIndex++;

                }
            }
        }
    }

    // -------------------------------------------- Memory Chunk logic ----------------------- //


    public void RemoveLastChunk()
    {
        if (chunkIndex > 0 && loadedPrefabScene)
        {
            if (chunkIndex == 1)
            {
                if (preLoadChunk)
                {
                    spawnedPrefabs.Remove(loadedPrefabScene);

                    Debug.LogWarning(loadedPrefabScene);

                    GameObject.Destroy(loadedPrefabScene);
                    loadedPrefabScene = null;

                    Resources.UnloadUnusedAssets();
                    GC.Collect();
                    chunkIndex = 0;
                }
                else
                {

                    spawnedPrefabs.Remove(loadedPrefabScene);

                    Debug.LogWarning(loadedPrefabScene);

                    GameObject.Destroy(loadedPrefabScene);
                    loadedPrefabScene = null;

                    Resources.UnloadUnusedAssets();
                    GC.Collect();

                    chunkIndex = 0;

                    Debug.LogWarning("preLoadChunk False: NOT last in list");
                }
            }
            else if (chunkIndex > 1)
            {
                chunkIndex--;

                if (preLoadChunk)
                {
                    spawnedPrefabs.Remove(loadedPrefabScene);

                    Debug.LogWarning(loadedPrefabScene);

                    GameObject.Destroy(loadedPrefabScene);
                    loadedPrefabScene = null;

                    Resources.UnloadUnusedAssets();
                    GC.Collect();

                    loadedPrefabScene = spawnedPrefabs[chunkIndex - 1].gameObject;
                }
                else
                {
                    spawnedPrefabs.Remove(loadedPrefabScene);

                    Debug.LogWarning(loadedPrefabScene);

                    GameObject.Destroy(loadedPrefabScene);
                    loadedPrefabScene = null;

                    Resources.UnloadUnusedAssets();
                    GC.Collect();

                    loadedPrefabScene = spawnedPrefabs[chunkIndex - 1].gameObject;

                    Debug.LogWarning("preLoadChunk False: NOT last in list");
                }
            } 
        }
    }

}
