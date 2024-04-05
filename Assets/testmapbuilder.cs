using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class testmapbuilder : MonoBehaviour
{
    public Tilemap map;
    // Start is called before the first frame update
    void Start()
    {
        ChunkData data = new ChunkData("test.xml");
        data.BuildToTileMap(map, Vector2Int.zero);
        data.BuildToTileMap(map, new Vector2Int(10,0));
        data.BuildToTileMap(map, new Vector2Int(20, 0));
        map.RefreshAllTiles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
