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

    // ---------------------------------------- Core Asset Logic ---------------------------- //

    public void AllocateChunksToMemory()
    {
        if (preLoadChunk)
        {
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

    public void SpawnNextChunk()
    {
        if (chunkIndex < assetBundleList.Count)
        {
            if (preLoadChunk)
            {
                loadedPrefabScene = Instantiate(scenePrefabs[chunkIndex].gameObject, scenePrefabs[chunkIndex].GetComponent<ChunkData>().scenePrefabPosition, scenePrefabs[chunkIndex].transform.rotation);
                spawnedPrefabs.Add(loadedPrefabScene);
                SetSpawnPositions();
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
                    SetSpawnPositions();
                    chunkIndex++;
                }
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
            if (spawnedPrefabs.Count != 0)
            {
                Debug.LogWarning("spawnedPrefabs.Count != 0");
                spawnedPrefabData.scenePrefabPosition =
                new Vector3(spawnedPrefabs[chunkIndex].GetComponent<ChunkData>().scenePrefabPosition.x + 100f, 0f, 0f);
            }
        }
        if (spawnedPrefabData.scenePrefabPosition == Vector3.zero)
        {
            Debug.LogWarning("The spawned prefab: " + loadedPrefabScene.name + " saved position is Vector(0,0,0).");
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
