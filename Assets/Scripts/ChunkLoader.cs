using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Events;

public class ChunkLoader : MonoBehaviour
{
    [SerializeField] ChunkSpawner chunkSpawner;
    public string[] NameOfAssetBundles;
    [Space]
    int assetIndexer;

    AssetBundleRequest chunkAssetRequest;
    List<AssetBundle> assetBundlesList = new List<AssetBundle>();

    UnityEvent AssetsEvent;
    
    void Awake()
    {
        AssetsEvent = new UnityEvent();
        AssetsEvent.AddListener(chunkSpawner.AllocateChunksToMemory);
        chunkSpawner = GetComponent<ChunkSpawner>();
    }

    //Comment everything from here down to the end of the for-loop to build the assetBundles.
    public void LoadAssets()
    {
        NameOfAssetBundles = AssetDatabase.GetAllAssetBundleNames();

        // These value didn't show to affect performance much, maybe it does.
        Application.backgroundLoadingPriority = ThreadPriority.Low;
        AssetBundle.memoryBudgetKB = 1024; 

        StartCoroutine(LoadAssetsAsync());
    }


    IEnumerator LoadAssetsAsync()
    {
        AssetBundleCreateRequest bundleLoadRequest = new AssetBundleCreateRequest();
        bundleLoadRequest = AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, NameOfAssetBundles[assetIndexer]));

        yield return bundleLoadRequest.isDone;

        chunkSpawner.assetBundleList.Add(bundleLoadRequest.assetBundle);
        assetIndexer++;

        if (assetIndexer != NameOfAssetBundles.Length) StartCoroutine(LoadAssetsAsync());
        else AssetsEvent.Invoke();
    }
}
