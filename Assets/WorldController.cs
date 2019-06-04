using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class WorldController : MonoBehaviour {
    private string  shroud = "./Assets/shroudcay.raw";
    private string  snowbasin = "./Assets/snowbasin514.raw";
	// Use this for initialization
	void Start () {

        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(snowbasin, terrain.terrainData);
	}
	
	 TerrainData GenerateTerrain(string aFileName, TerrainData aTerrain)
 {
     float max = 0;
     float min = 1;
     int count = 0;

     int h = aTerrain.heightmapHeight;
     int w = aTerrain.heightmapWidth;
     float[,] data = new float[h, w];
     Debug.Log("h="+h + "   &  w="+w);
     using (var file = System.IO.File.OpenRead(aFileName))
     using (var reader = new System.IO.BinaryReader(file))
    
     {
         for (int y = 0; y < h; y++)
         {
             for (int x = 0; x < w; x++)
             {
                 Debug.Log("x="+ x + " , y="+y);
                 float v = (float)reader.ReadUInt16() / 0xFFFF;
                 data[y, x] = v / 100   ;
                 if( v > max) {
                     max = v;
                 }
                 if (v < min) {
                     min = v;
                 }
             }
         }
     }
     aTerrain.SetHeights(0, 0, data);
     
     Debug.Log("max is "+ max);
      Debug.Log("min is "+ min);
     return aTerrain;
 }
	// Update is called once per frame
	void Update () {
		
	}
}
