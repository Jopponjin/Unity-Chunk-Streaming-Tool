using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class Chunk : MonoBehaviour
{
    public GameObject chunkPrefab;
    public Vector3 chunkPostion;
}

public class ChunkSpawner : MonoBehaviour
{
    public List<AssetBundle> assetBundleList = new List<AssetBundle>();
    public List<Chunk> chunkOrder = new List<Chunk>();
    public List<GameObject> scenePrefabs = new List<GameObject>();
    [Space]
    public Vector3[] spawnPostions;
    [Space]
    public int chunkIndex = 0;
    [Space]
    public Chunk lastChunkSpawned;
    [Space]
    public GameObject loadedPrefabScene;
    [Space]
    public Transform chunkSpawnPosition;
    [Space]
    public bool preLoadChunk = false;
    public int preloadAmountToMemory = 2;

    void Start()
    {
        loadedPrefabScene = null;

        string[] assets = AssetDatabase.GetAllAssetBundleNames();
        

        for (int i = 0; i < assets.Length; i++)
        {
            assetBundleList.Add(AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, assets[i])));   
        }

        AllocateChunksToMemory();

        SetSpawnPositions();
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
                chunkOrder.Add(new Chunk());
                chunkOrder[i].chunkPrefab = scenePrefab;
            }
            else
            {
                Debug.Log("LoadAssetsFromBundleAsync assetGameObject IS null!");
            }
        }

    }


    public void SetSpawnPositions()
    {
        if (preLoadChunk)
        {
            for (int i = 0; i < assetBundleList.Count; i++)
            {
                chunkOrder[i].chunkPostion = spawnPostions[i];
            }
        }
    }

    public void SpawnNextChunk()
    {
        if (preLoadChunk)
        {
            loadedPrefabScene = Instantiate(chunkOrder[chunkIndex].chunkPrefab.gameObject, chunkOrder[chunkIndex].chunkPostion, chunkOrder[chunkIndex].chunkPrefab.transform.rotation);
            lastChunkSpawned = chunkOrder[chunkIndex];
            scenePrefabs.Add(loadedPrefabScene);
            chunkIndex++;
        }
        else
        {
            loadedPrefabScene = assetBundleList[chunkIndex].LoadAllAssets().GetValue(0) as GameObject;

            Debug.Log(loadedPrefabScene.name + " is being loaded from assets.");

            if (loadedPrefabScene)
            {
                Debug.Log(loadedPrefabScene.name + " Is loaded to memory.");

                Chunk newChunk = new Chunk();

                chunkOrder.Insert(chunkIndex, newChunk);
                chunkOrder[chunkIndex].chunkPrefab = loadedPrefabScene;

                loadedPrefabScene = Instantiate(chunkOrder[chunkIndex].chunkPrefab, spawnPostions[chunkIndex], chunkOrder[chunkIndex].chunkPrefab.transform.rotation);
                lastChunkSpawned = chunkOrder[chunkIndex];
                scenePrefabs.Add(loadedPrefabScene);
                chunkIndex++;

            }
        }
    }

    // -------------------------------------------- Some Chunk logic.. I don't know ----------------------- //

    bool CheckAllChunks(Chunk[] chunkArrayToCheck)
    {
        int numOfChunkNotNull = 0;

        for (int i = 0; i < chunkArrayToCheck.Length; i++)
        {
            if (chunkArrayToCheck[i]) numOfChunkNotNull++;
            else numOfChunkNotNull--;
        }

        if (numOfChunkNotNull == chunkArrayToCheck.Length) return true;
        else return false;
    }

    public bool CheckAChunk(Chunk chunkToCheck)
    {
        if (chunkToCheck)
        {
            return true;
        }
        else return false;
    }

    public void RemoveLastChunk()
    {
        if (chunkIndex > 0)
        {
            chunkIndex--;

            if (chunkOrder[chunkIndex] != null)
            {
                
                if (preLoadChunk)
                {
                    GameObject.Destroy(scenePrefabs[chunkIndex]);
                    scenePrefabs.Remove(scenePrefabs[chunkIndex]);
                    loadedPrefabScene = chunkOrder[chunkIndex].gameObject;
                }
                else
                {
                    GameObject.Destroy(scenePrefabs[chunkIndex]);
                    scenePrefabs.Remove(scenePrefabs[chunkIndex]);
                    loadedPrefabScene = chunkOrder[chunkIndex].gameObject;

                    chunkOrder.RemoveAt(chunkIndex);
                }
            }
            else if (chunkOrder[chunkIndex] == null)
            {
                if (preLoadChunk)
                {
                    GameObject.Destroy(scenePrefabs[chunkIndex]);
                    scenePrefabs.Remove(scenePrefabs[chunkIndex]);
                    loadedPrefabScene = null;
                }
                else
                {
                    GameObject.Destroy(scenePrefabs[chunkIndex]);
                    scenePrefabs.Remove(scenePrefabs[chunkIndex]);
                    loadedPrefabScene = null;

                    chunkOrder.RemoveAt(chunkIndex);
                }
            }
        }
    }

}
