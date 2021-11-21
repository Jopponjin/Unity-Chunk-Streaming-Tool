using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Events;

public class ChunkLoader : MonoBehaviour
{
    public ChunkSpawner chunkSpawner;
    public string[] NameOfAssetBundles;

    int assetIndexer;
    AssetBundleRequest chunkAssetRequest;
    List<AssetBundle> assetBundlesList = new List<AssetBundle>();
    UnityEvent unityEvent;
    
    // Start is called before the first frame update
    void Awake()
    {
        unityEvent = new UnityEvent();
        unityEvent.AddListener(chunkSpawner.AllocateChunksToMemory);

        chunkSpawner = GetComponent<ChunkSpawner>();

        NameOfAssetBundles = AssetDatabase.GetAllAssetBundleNames();
    }

    public void LoadAssets()
    {
        //Comment everything from here down to the end of the for-loop to build the assetBundles.
        Application.backgroundLoadingPriority = ThreadPriority.Low;
        AssetBundle.memoryBudgetKB = 5024;

        StartCoroutine(LoadAssetsAsync());
        Debug.LogWarning("LoadAssetAsync");
    }

    IEnumerator LoadAssetsAsync()
    {
        AssetBundleCreateRequest bundleLoadRequest = new AssetBundleCreateRequest();

        bundleLoadRequest = AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, NameOfAssetBundles[assetIndexer]));

        yield return bundleLoadRequest.isDone;

        chunkSpawner.assetBundleList.Add(bundleLoadRequest.assetBundle);

        assetIndexer++;

        if (assetIndexer != NameOfAssetBundles.Length)
        {
            StartCoroutine(LoadAssetsAsync());
        }
        else
        {
            unityEvent.Invoke();
        }
    }

}
