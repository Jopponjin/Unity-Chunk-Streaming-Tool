using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class ChunkSpawner : MonoBehaviour
{
    public List<AssetBundle> assetBundleList;
    [SerializeField] List<GameObject> scenePrefabs = new List<GameObject>();
    [SerializeField] List<GameObject> spawnedPrefabs = new List<GameObject>();

    [SerializeField] List<Vector3> spawnPostions = new List<Vector3>();
    [Space]
    [SerializeField] int chunkIndex = 0;
    [Space]
    [SerializeField] GameObject loadedPrefabScene;
    [Space]
    [SerializeField] bool preLoadChunk = false;
    [SerializeField] bool purgeByTrigger = false;
    [Space]
    [SerializeField] bool overideSetSpawnPoints = true;


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
        if (assetBundleList.Count == 0)
        {
            Debug.LogWarning("assetBundleList.Count is 0!");
        }
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
                loadedPrefabScene = Instantiate(scenePrefabs[chunkIndex].gameObject, scenePrefabs[chunkIndex].gameObject.transform.position, scenePrefabs[chunkIndex].transform.rotation);
                spawnedPrefabs.Add(loadedPrefabScene);
                SetSpawnPositions();
                chunkIndex++;
            }
            else
            {
                loadedPrefabScene = assetBundleList[chunkIndex].LoadAllAssets().GetValue(0) as GameObject;

                if (loadedPrefabScene)
                {
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
            if (loadedPrefabScene.GetComponent<ChunkData>().isMemoryClearChunk && purgeByTrigger)
            {
                PurgeLoadedChunks();
            }
        }
    }

    // Any prefabs spawned to dont get affected by the SetSpawnPositions() logic if they have Postitions set in ChunkData.cs
    public void SetSpawnPositions()
    {
        ChunkData spawnedPrefabData = loadedPrefabScene.GetComponent<ChunkData>();

        if (!overideSetSpawnPoints && spawnPostions[chunkIndex] != Vector3.zero)
        {
            loadedPrefabScene.transform.position = spawnPostions[chunkIndex];
        }
        else if(!overideSetSpawnPoints)
        {
            if (chunkIndex > 1)
            {
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
            spawnedPrefabData.transform.position =
                new Vector3(spawnedPrefabs[chunkIndex - 1].GetComponent<ChunkData>().scenePrefabPosition.x + 100f, 0f, 0f);
        }
    }

    // -------------------------------------------- Memory Mangement logic ----------------------- //

    public void PurgeLoadedChunks()
    {
        GameObject savedCurrentScene = loadedPrefabScene;

        for (int i = 0; i < spawnedPrefabs.Count; i++)
        {
            if (spawnedPrefabs[i] != savedCurrentScene && spawnedPrefabs[i] != null)
            {
                loadedPrefabScene = spawnedPrefabs[i];

                Destroy(loadedPrefabScene);
            }
        }
        Resources.UnloadUnusedAssets();
        GC.Collect();
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
