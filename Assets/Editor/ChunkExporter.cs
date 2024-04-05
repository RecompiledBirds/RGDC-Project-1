using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

[CustomEditor(typeof(Tilemap))]
public class ChunkExporter : Editor
{
    private bool showChunkOptions;
    private Vector2Int startingPos;
    private Vector2Int size= new Vector2Int(10,10);
    private ChunkType chunkType;
    private string chunkId;
    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("Export map as chunk"))
        {
            ExportChunk();
        }
        if (showChunkOptions)
        {
            GUILayout.Label("Chunk Starting pos");
            EditorGUILayout.BeginHorizontal();
            startingPos.x = EditorGUILayout.IntField(startingPos.x);
            startingPos.y = EditorGUILayout.IntField(startingPos.y);
            EditorGUILayout.EndHorizontal();
            GUILayout.Label("Chunk size");
            EditorGUILayout.BeginHorizontal();
            size.x = EditorGUILayout.IntField(size.x);
            size.y = EditorGUILayout.IntField(size.y);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        if(EditorGUILayout.DropdownButton(new GUIContent("Show chunk options"), FocusType.Keyboard)) { showChunkOptions = !showChunkOptions; }
        if(showChunkOptions)
        {
            chunkType= (ChunkType)EditorGUILayout.EnumPopup(chunkType);
            GUILayout.Label("Chunk id");
            chunkId = GUILayout.TextField(chunkId);

          
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        base.OnInspectorGUI();

    }


    private void OnSceneGUI()
    {
        Handles.color = Color.green;
        BoundsInt bounds = new BoundsInt(new Vector3Int(startingPos.x,startingPos.y), new Vector3Int(size.x,size.y));
        Handles.DrawWireCube(bounds.center,bounds.size);
    }
    private void ExportChunk()
    {
        ChunkData data = new ChunkData(chunkType);
        Tilemap map = (Tilemap)target;

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
}
