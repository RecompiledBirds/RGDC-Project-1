using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.Windows;
using Directory = System.IO.Directory;

public class ChunkData
{
    public static string ChunkFolder
    {
        get
        {
            return $"{Application.dataPath}/Chunks/";
        }
    }
    public ChunkData(ChunkType type)
    {
        this.type = type;
    }

    public ChunkType Type
    {
        get
        {
            return this.type;
        }
    }

    public Vector2Int MaxXY { get { return maxXY; } }
    private Vector2Int maxXY = Vector2Int.zero;
    public Vector2Int MinXY { get { return minXY; } }
    private Vector2Int minXY = Vector2Int.zero;


    /// <summary>
    /// Builds data from the chunk into the tilemap.
    /// Remember to refresh the map afterwards!
    /// </summary>
    /// <param name="tilemap"></param>
    /// <param name="offset"></param>
    public void BuildToTileMap(Tilemap tilemap,Vector2Int offset)
    {
        foreach(Vector2Int pos in tileData.Keys)
        {
            Tile tile =ResourceManager<Tile>.LoadResource(tileData[pos]);
            tilemap.SetTile(new Vector3Int(pos.x+offset.x,pos.y+offset.y,0), tile);
        }
    }
    public ChunkData(string file,bool xmlEnding = false, bool isCompletePath= true)
    {
        XmlDocument fileDoc= new XmlDocument();
        fileDoc.Load($"{(isCompletePath? "" : ChunkFolder+"/")}{file}{(xmlEnding?".xml":"")}");
        XmlNode root = fileDoc.DocumentElement;
        XmlNode typeNode = root["chunkType"];
        string typeVal = typeNode.InnerText;
        type= (ChunkType)Enum.Parse(typeof(ChunkType), typeVal);
        //load id
        id = root["chunkId"].InnerText;
       

        //Decompress the list and load it into a new xml doc
        XmlDocument decompressedDoc = new XmlDocument();
        decompressedDoc.LoadXml(BasicCompressor.DecompressString(root["compressedDataList"].InnerText));
        XmlNode decompressed_dict_root = decompressedDoc.DocumentElement;
       
        foreach(XmlNode kvp_child in decompressed_dict_root.ChildNodes)
        {
            //parse the stored vec2int back to a string
            Vector2Int pos = Vector2FromXMLNode(kvp_child);
            if(pos.x>MaxXY.x)maxXY.x = pos.x;
            if (pos.y > MaxXY.y) maxXY.y = pos.y;
            if (pos.x < MinXY.x) minXY.x = pos.x;
            if (pos.y < MinXY.y) minXY.y = pos.y;
            //get tile
            string tile = kvp_child["tile"].InnerText;

            tileData.Add(pos,tile);

        }

        XmlDocument decompressedDoc_bg = new XmlDocument();
        decompressedDoc_bg.LoadXml(BasicCompressor.DecompressString(root["compressedBGData"].InnerText));
        XmlNode decompressed_dict_root_bg = decompressedDoc.DocumentElement;

        foreach (XmlNode kvp_child in decompressed_dict_root_bg.ChildNodes)
        {
            //parse the stored vec2int back to a string
            Vector2Int pos = Vector2FromXMLNode(kvp_child);
            //get tile
            string tile = kvp_child["tile"].InnerText;

            backgroundTileData.Add(pos, tile);

        }

    }

    private static Vector2Int Vector2FromXMLNode(XmlNode kvp_child)
    {
        string pos_string = kvp_child["pos"].InnerText;
        string trimmed = pos_string.Trim('(', ')');
        string[] parts = trimmed.Split(',');
        int x = int.Parse(parts[0]);
        int y = int.Parse(parts[1]);

        Vector2Int pos = new Vector2Int(x, y);
        return pos;
    }
#if UNITY_EDITOR
    /// <summary>
    /// In the editor, we need a way to convert chunkdata to xml
    /// 
    /// This method writes XML to a given file.
    /// </summary>
    /// <param name="chunkFileName"></param>
    public void ChunkDataToXML(string chunkFileName)
    {
        if (!Directory.Exists(ChunkFolder)) Directory.CreateDirectory(ChunkFolder);
        XmlWriter writer = XmlWriter.Create($"{ChunkFolder}/{chunkFileName}.xml", new XmlWriterSettings() { Indent=true});
        writer.WriteStartDocument();
        writer.WriteStartElement("chunkdata");
        WriteNode(writer, "chunkId", chunkFileName);
        WriteNode(writer, "chunkType",type.ToString());

        //Compress the giant list of posititions and tiledata
        StringWriter stringWriter = new StringWriter();
        XmlWriter listWriter = XmlWriter.Create(stringWriter);
        listWriter.WriteStartElement("chunkDataDict");
        foreach (Vector2Int key in tileData.Keys)
        {
            listWriter.WriteStartElement("kvp");

            WriteNode(listWriter, "pos", key.ToString());
            WriteNode(listWriter, "tile", tileData[key]);
            listWriter.WriteEndElement();
        }
        listWriter.WriteEndElement();
        listWriter.Close();
        string list_final = stringWriter.ToString();
        string compressed = BasicCompressor.CompressString(list_final);

        stringWriter.Close();
        WriteNode(writer, "compressedDataList", compressed);

        //Compress the giant list of posititions and tiledata for BG
        stringWriter = new StringWriter();
        listWriter = XmlWriter.Create(stringWriter);
        listWriter.WriteStartElement("chunkBGDataDict");
        foreach (Vector2Int key in backgroundTileData.Keys)
        {
            listWriter.WriteStartElement("kvp");

            WriteNode(listWriter, "pos", key.ToString());
            WriteNode(listWriter, "tile", backgroundTileData[key]);
            listWriter.WriteEndElement();
        }
        listWriter.WriteEndElement();
        listWriter.Close();
        string list_final_bg = stringWriter.ToString();
        string compressed_bg = BasicCompressor.CompressString(list_final_bg);


        stringWriter.Close();
        WriteNode(writer, "compressedBGData", compressed_bg);

        

        writer.WriteEndElement();
        writer.Flush();
        writer.Close();

    }

    private void WriteNode(XmlWriter writer, string name, string value)
    {
        writer.WriteStartElement(name);
        writer.WriteValue(value);
        writer.WriteEndElement();
    }


#endif
    private string id = "";
    private ChunkType type;
   
    public Dictionary<Vector2Int, string> tileData = new Dictionary<Vector2Int, string>();
    public Dictionary<Vector2Int, string> backgroundTileData = new Dictionary<Vector2Int, string>();
}
