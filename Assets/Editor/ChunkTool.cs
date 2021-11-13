using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

//[CustomEditor(typeof(ChunkSpawner))]
public class ChunkTool : Editor
{
    ChunkSpawner chunkSpawner;
    private GUISkin sceneSkin;
    private GUISkin inspectorSkin;

    public string[] chunkAssets;

    // Start is called before the first frame update



    private void OnEnable()
    {
        
        sceneSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);
        inspectorSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
        Tools.hidden = false;
        chunkSpawner = target as ChunkSpawner;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.BeginHorizontal();



        EditorGUI.BeginChangeCheck();
        if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Plus"), GUILayout.ExpandWidth(true)))
        {

        }
        EditorGUI.EndChangeCheck();



        GUILayout.EndHorizontal();
        EditorGUILayout.Space();


        GUILayout.BeginVertical();


        GUILayout.EndVertical();
    }

}
