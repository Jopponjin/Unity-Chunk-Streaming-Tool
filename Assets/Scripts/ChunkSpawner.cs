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

    public Vector3[] spawnPostions;
    [Space]
    public int chunkIndex = 0;
    [Space]
    public Chunk lastChunkSpawned;
    public GameObject latestSpawnedChunk;
    public GameObject loadedPrefabScene;
    [Space]
    public Transform chunkSpawnPosition;
    [Space]
    public bool preLoadChunk = false;
    public int preloadAmountToMemory = 2;

    void Start()
    {

        loadedPrefabScene = null;
        latestSpawnedChunk = null;

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
            latestSpawnedChunk = assetBundleList[i].LoadAllAssets().GetValue(0) as GameObject;

            if (latestSpawnedChunk)
            {
                Chunk newChunk = new Chunk();

                chunkOrder.Insert(i, newChunk);
                chunkOrder[i].chunkPrefab = latestSpawnedChunk;
            }
            else
            {
                Debug.Log("LoadAssetsFromBundleAsync assetGameObject IS null!");
            }
        }

    }


    public void SetSpawnPositions()
    {
        for (int i = 0; i < assetBundleList.Count; i++)
        {
            chunkOrder[i].chunkPostion = spawnPostions[i];
        }
    }

    public void SpawnNextChunk()
    {
        if (chunkOrder[chunkIndex].chunkPrefab && preLoadChunk)
        {
            latestSpawnedChunk = Instantiate(chunkOrder[chunkIndex].chunkPrefab.gameObject, chunkOrder[chunkIndex].chunkPostion, chunkOrder[chunkIndex].chunkPrefab.transform.rotation);
            lastChunkSpawned = chunkOrder[chunkIndex];
            chunkIndex++;
        }
        else
        {
            GameObject chunkToSpawn = new GameObject();

            latestSpawnedChunk = assetBundleList[chunkIndex].LoadAllAssets().GetValue(0) as GameObject;

            if (latestSpawnedChunk)
            {
                Debug.Log(latestSpawnedChunk.name + " Is loaded to memory.");

                Chunk newChunk = new Chunk();

                chunkOrder.Insert(chunkIndex, newChunk);
                chunkOrder[chunkIndex].chunkPrefab = latestSpawnedChunk;

                latestSpawnedChunk = Instantiate(chunkOrder[chunkIndex].chunkPrefab, chunkOrder[chunkIndex].chunkPostion, chunkOrder[chunkIndex].chunkPrefab.transform.rotation);
                lastChunkSpawned = chunkOrder[chunkIndex];
                chunkIndex++;

            }
            else
            {
                chunkToSpawn = null;
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
        chunkIndex--;
        GameObject.Destroy(latestSpawnedChunk);
        lastChunkSpawned = chunkOrder[chunkIndex];
    }

    public void RemoveChunk(string chunkName)
    {
        if (chunkName != "")
        {
            GameObject.Destroy(latestSpawnedChunk);
            loadedPrefabScene = null;
        }
    }

}
