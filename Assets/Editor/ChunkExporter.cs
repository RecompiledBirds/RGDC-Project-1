using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class ChunkExporter : EditorWindow
{
    private Vector2Int startingPos;
    private Vector2Int size = new Vector2Int(10, 10);
    private ChunkType chunkType;
    private string chunkId;
    private static Tilemap map;
    private static Tilemap background;
    private void ExportChunk()
    {
        ChunkData data = new ChunkData(chunkType);

        for (int x = startingPos.x; x <= startingPos.x + size.x; x++)
        {
            for (int y = startingPos.y; y <= startingPos.y + size.y; y++)
            {
                Vector3Int pos = new Vector3Int(x, y);
                pos = map.WorldToCell(pos);
                TileBase tile = map.GetTile(pos);

                if (tile == null) continue;
                data.tileData.Add(new Vector2Int(pos.x, pos.y), tile.name);


            }
        }

        data.ChunkDataToXML(chunkId);


    }

    public void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Map");
        map = (Tilemap)EditorGUILayout.ObjectField(map, typeof(Tilemap), true);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Background");
        background = (Tilemap)EditorGUILayout.ObjectField(background, typeof(Tilemap), true);
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Export map as chunk"))
        {
            ExportChunk();
        }

        GUILayout.Label("Chunk Starting pos");
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("X");
        startingPos.x = EditorGUILayout.IntField(startingPos.x);
        GUILayout.Label("Y");
        startingPos.y = EditorGUILayout.IntField(startingPos.y);
        EditorGUILayout.EndHorizontal();
        GUILayout.Label("Chunk size");
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("X");
        size.x = EditorGUILayout.IntField(size.x);
        GUILayout.Label("Y");
        size.y = EditorGUILayout.IntField(size.y);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Type");
        chunkType = (ChunkType)EditorGUILayout.EnumPopup(chunkType);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Chunk id");
        chunkId = GUILayout.TextField(chunkId);
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    
    [MenuItem("RUGDC TOOLS / Chunk Exporter")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ChunkExporter));
    }
}
