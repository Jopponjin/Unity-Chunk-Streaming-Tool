using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class StreamingAssets : MonoBehaviour
{
    DirectoryInfo directoryInfo = new DirectoryInfo(Application.streamingAssetsPath);



    // Update is called once per frame
    void Awake()
    {
        FileInfo[] allFiles = directoryInfo.GetFiles("*.*");

        for (int i = 0; i < allFiles.Length; i++)
        {
            Debug.Log(allFiles[i].Name + " is in StreamingAsset folder.");
        }

    }


    
}
