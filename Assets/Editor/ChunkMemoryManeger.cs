using System;
using UnityEditor;
using UnityEngine;

public class ChunkMemoryManeger : MonoBehaviour
{
    

    public void Start()
    {


    }

    static void GarbageCollect()
    {
        EditorUtility.UnloadUnusedAssetsImmediate();
        GC.Collect();
    }
}
