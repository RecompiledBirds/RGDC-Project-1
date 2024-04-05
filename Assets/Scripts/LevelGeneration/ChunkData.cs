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

public class ChunkData
{
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
    public ChunkData(string file)
    {
        XmlDocument fileDoc= new XmlDocument();
        fileDoc.Load(file);
        XmlNode root = fileDoc.DocumentElement;

        //load id
        id = root["chunkId"].InnerText;
       

        //Decompress the list and load it into a new xml doc
        XmlDocument decompressedDoc = new XmlDocument();
        decompressedDoc.LoadXml(BasicCompressor.DecompressString(root["compressedDataList"].InnerText));
        XmlNode decompressed_dict_root = decompressedDoc.DocumentElement;
        foreach(XmlNode kvp_child in decompressed_dict_root.ChildNodes)
        {
            //parse the stored vec2int back to a string
            string pos_string = kvp_child["pos"].InnerText;
            string trimmed = pos_string.Trim('(', ')');
            string[] parts = trimmed.Split(',');
            int x = int.Parse(parts[0]);
            int y = int.Parse(parts[1]);

            Vector2Int pos = new Vector2Int(x, y);
            //get tile
            string tile = kvp_child["tile"].InnerText;

            tileData.Add(pos,tile);

        }
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
        XmlWriter writer = XmlWriter.Create($"{chunkFileName}.xml", new XmlWriterSettings() { Indent=true});
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


        WriteNode(writer, "compressedDataList", compressed);
        stringWriter.Close();
        

        

        writer.WriteEndElement();
        writer.Flush();
        writer.Close();


        ChunkData d = new ChunkData($"{chunkFileName}.xml");
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
}
