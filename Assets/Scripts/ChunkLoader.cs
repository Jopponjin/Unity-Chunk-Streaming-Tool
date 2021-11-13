using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ChunkLoader : MonoBehaviour
{
    public ChunkSpawner chunkSpawner;
    public string[] chunkAssets;

    // Start is called before the first frame update
    void Awake()
    {
        chunkSpawner = GetComponent<ChunkSpawner>();
    }

    public void LoadAssets()
    {
        //Comment everything from here down to the end of the for-loop to build the assetBundles.

        string[] assetBundles = AssetDatabase.GetAllAssetBundleNames();



        for (int i = 0; i < assetBundles.Length; i++)
        {
            if (assetBundles[i].Contains("chunk"))
            {
                Debug.Log("Added asset to assetBundleList!");

                chunkSpawner.assetBundleList.Add(AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, assetBundles[i])));
            }
        }


        chunkSpawner.AllocateChunksToMemory();

        chunkSpawner.SetSpawnPositions();
    }
}
