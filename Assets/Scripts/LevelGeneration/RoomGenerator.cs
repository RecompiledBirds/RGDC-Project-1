using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
public class RoomGenerator : MonoBehaviour
{
    public int roomCount;

    private List<ChunkData> chunks = new List<ChunkData>();
    Dictionary<ChunkType, List<ChunkData>> chunksDict = new Dictionary<ChunkType, List<ChunkData>>();
    public Tilemap level;
    // Start is called before the first frame update

    void Start()
    {
        //Init all chunklists
        chunksDict[ChunkType.START] = new List<ChunkData>();
        chunksDict[ChunkType.MIDDLE] = new List<ChunkData>();
        chunksDict[ChunkType.END] = new List<ChunkData>();
        //find all directories
        string startDir = ChunkData.ChunkFolder;
        Queue<string> directoryQueue = new Queue<string>();
        directoryQueue.Enqueue(startDir);
        List<string> directoryList = new List<string>();
        while (directoryQueue.Count > 0)
        {
            string dir = directoryQueue.Dequeue();
            string[] newDirs =Directory.GetDirectories(dir);
            foreach(string newDir in newDirs)
            {
                directoryQueue.Enqueue(newDir);
            }
            directoryList.Add(dir);
        }

        //Load chunks from all directories
        int chunksLoaded = 0;
        foreach(string directory in directoryList)
        {
            foreach(string file in Directory.GetFiles(directory,"*.xml"))
            {
                ChunkData chunkData = new ChunkData(file);
                chunks.Add(chunkData);
                chunksDict[chunkData.Type].Add(chunkData);
                chunksLoaded++;
            }
        }
        Debug.Log($"Loaded {chunksLoaded} chunks | {chunksDict[ChunkType.START].Count} start chunks | {chunksDict[ChunkType.MIDDLE].Count} middle chunks | {chunksDict[ChunkType.END].Count} end chunks");
        GenerateRooms();
    }

    public void GenerateRooms()
    {
        int offset = 0;
        ChunkData start = chunksDict[ChunkType.START].RandomElement();
        start.BuildToTileMap(level, new Vector2Int(offset, 0));
        offset += start.MaxXY.x;
        for(int i =0; i<roomCount; i++)
        {
            ChunkData newChunk = chunksDict[ChunkType.MIDDLE].RandomElement();
            newChunk.BuildToTileMap(level, new Vector2Int(offset, 0));
            offset += newChunk.MaxXY.x;
        }
        ChunkData end = chunksDict[ChunkType.END].RandomElement();
        end.BuildToTileMap(level, new Vector2Int(offset, 0));

    }
}
