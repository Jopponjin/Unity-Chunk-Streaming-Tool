using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class ChunkSpawner : MonoBehaviour
{
    public List<AssetBundle> assetBundleList;
    public List<GameObject> scenePrefabs = new List<GameObject>();
    public List<GameObject> spawnedPrefabs = new List<GameObject>();

    public List<Vector3> spawnPostions = new List<Vector3>();
    [Space]
    public int chunkIndex = 0;
    [Space]
    public GameObject loadedPrefabScene;
    [Space]
    public bool preLoadChunk = false;

    public bool preLoadByTrigger = false;
    public bool purgeByTrigger = false;
    [Space]
    public int preLoadChunkAmount = 5;
    [Space]
    public bool overideSetSpawnPoints = true;

    // ---------------------------------------- Core Asset Logic ---------------------------- //

    public void AllocateChunksToMemory()
    {
        if (preLoadChunk)
        {
            Debug.LogWarning("LoadChunksFromAssets called!");
            LoadChunksFromAssets();
        }
        for (int i = 0; i < assetBundleList.Count; i++)
        {
            if (spawnPostions.Count == 0)
            {
                spawnPostions.Add(new Vector3());
                spawnPostions[i] = Vector3.zero;
            }
            else if(i == spawnPostions.Count)
            {
                spawnPostions.Add(new Vector3());
                spawnPostions[i] = Vector3.zero;
            }
        }
    }


    public void LoadChunksFromAssets()
    {
        Debug.LogWarning("LoadChunksFromAssets has been called!");

        if (assetBundleList.Count == 0)
        {
            Debug.LogWarning("assetBundleList.Count is 0!");
        }

        for (int i = 0; i < assetBundleList.Count; i++)
        {
            var scenePrefab = assetBundleList[i].LoadAllAssets().GetValue(0) as GameObject;

            Debug.LogWarning("assetBundleList[i].LoadAllAssets() called!");

            if (scenePrefab)
            {
                
                scenePrefabs.Add(scenePrefab);
                Debug.Log(scenePrefab.name + "is being added to List!");
            }
            else
            {
                Debug.Log("LoadChunksFromAssets GameObject IS null!");
            }
        }
        

    }

    public void SpawnNextChunk()
    {
        if (chunkIndex < assetBundleList.Count)
        {
            if (preLoadChunk)
            {
                loadedPrefabScene = Instantiate(scenePrefabs[chunkIndex].gameObject, scenePrefabs[chunkIndex].gameObject.transform.position, scenePrefabs[chunkIndex].transform.rotation);
                spawnedPrefabs.Add(loadedPrefabScene);
                SetSpawnPositions();
                chunkIndex++;
                Debug.Log(loadedPrefabScene.name + " is being loaded from assets.");
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
                    SetSpawnPositions();
                    chunkIndex++;
                }
            }
            if (loadedPrefabScene.GetComponent<ChunkData>().isMemoryClearChunk)
            {
                PurgeLoadedChunks();
            }
        }
    }


    public void SetSpawnPositions()
    {
        ChunkData spawnedPrefabData = loadedPrefabScene.GetComponent<ChunkData>();

        if (!overideSetSpawnPoints && spawnPostions[chunkIndex] != Vector3.zero)
        {
            loadedPrefabScene.transform.position = spawnPostions[chunkIndex];

            Debug.Log(loadedPrefabScene.name + "!overideSetSpawnPoints && spawnPostions[chunkIndex] != null");
        }
        else if(!overideSetSpawnPoints)
        {
            if (chunkIndex > 1)
            {
                Debug.LogWarning("The spawned prefab: " + loadedPrefabScene.name + " saved position is Vector(0,0,0).");
                spawnedPrefabData.transform.position =
                new Vector3(spawnedPrefabs[chunkIndex - 1].GetComponent<ChunkData>().scenePrefabPosition.x + 100f, 0f, 0f);
            }
            else
            {
                spawnedPrefabData.transform.position = new Vector3( 100f, 0f, 0f);
            }
        }
        if (spawnedPrefabData.scenePrefabPosition == Vector3.zero && spawnedPrefabs.Count != 0)
        {
            Debug.LogWarning("The spawned prefab: " + loadedPrefabScene.name + " saved position is Vector(0,0,0).");
            spawnedPrefabData.transform.position =
                new Vector3(spawnedPrefabs[chunkIndex - 1].GetComponent<ChunkData>().scenePrefabPosition.x + 100f, 0f, 0f);
        }
    }

    // -------------------------------------------- Memory Chunk logic ----------------------- //

    public void PurgeLoadedChunks()
    {
        GameObject savedCurrentScene = loadedPrefabScene;

        for (int i = 0; i < spawnedPrefabs.Count; i++)
        {
            if (spawnedPrefabs[i] != savedCurrentScene)
            {
                loadedPrefabScene = spawnedPrefabs[i];

                GameObject.Destroy(loadedPrefabScene);

                Debug.LogWarning(loadedPrefabScene.name + " has been pruged!");
            }
        }

        Resources.UnloadUnusedAssets();
        GC.Collect();

        if (preLoadChunk)
        {
            for (int i = 0; i < spawnedPrefabs.Count; i++)
            {
                if (spawnedPrefabs[i] == null)
                {
                    spawnedPrefabs.RemoveAt(i);
                }
            }
        }
    }

    public void RemoveLastChunk()
    {
        if (chunkIndex > 0 && loadedPrefabScene)
        {
            if (chunkIndex == 1)
            {
                if (preLoadChunk)
                {
                    spawnedPrefabs.Remove(loadedPrefabScene);

                    GameObject.Destroy(loadedPrefabScene);
                    loadedPrefabScene = null;

                    Resources.UnloadUnusedAssets();
                    GC.Collect();
                    chunkIndex = 0;
                }
                else
                {
                    spawnedPrefabs.Remove(loadedPrefabScene);

                    GameObject.Destroy(loadedPrefabScene);
                    loadedPrefabScene = null;

                    Resources.UnloadUnusedAssets();
                    GC.Collect();

                    chunkIndex = 0;
                }
            }
            else if (chunkIndex > 1)
            {
                chunkIndex--;

                if (preLoadChunk)
                {
                    spawnedPrefabs.Remove(loadedPrefabScene);

                    GameObject.Destroy(loadedPrefabScene);
                    loadedPrefabScene = null;

                    Resources.UnloadUnusedAssets();
                    GC.Collect();

                    loadedPrefabScene = spawnedPrefabs[chunkIndex - 1].gameObject;
                }
                else
                {
                    spawnedPrefabs.Remove(loadedPrefabScene);

                    GameObject.Destroy(loadedPrefabScene);
                    loadedPrefabScene = null;

                    Resources.UnloadUnusedAssets();
                    GC.Collect();

                    loadedPrefabScene = spawnedPrefabs[chunkIndex - 1].gameObject;
                }
            } 
        }
    }
}
