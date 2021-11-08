using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ChunkSpawner))]
public class ChunkTool : Editor
{
    ChunkSpawner chunkSpawner;
    private GUISkin sceneSkin;
    private GUISkin inspectorSkin;

    public Chunk[] ChunkOrder;

    private void OnEnable()
    {
        chunkSpawner = target as ChunkSpawner;
        sceneSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);
        inspectorSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
        Tools.hidden = false;
        
    }

    private void OnValidate()
    {
        
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
