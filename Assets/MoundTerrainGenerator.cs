using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MoundTerrainGenerator : MonoBehaviour {
	public int width = 500;
    public int height = 500;
    public int depth = 20;

	public float a;
	public float sigma;

	void Start () {
		Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
	}
	
	void Update () {
		
	}

	 TerrainData GenerateTerrain (TerrainData terrainData)
    {
        terrainData.size= new Vector3(width, depth, height);

        terrainData.SetHeights(0, 0, InitHeights());
 
        // Splatmap data is stored internally as a 3d array of floats, so declare a new empty array ready for your custom splatmap data:
        float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                // Normalise x/y coordinates to range 0-1 
                float y_01 = (float)y / (float)terrainData.alphamapHeight;
                float x_01 = (float)x / (float)terrainData.alphamapWidth;

                float height = terrainData.GetHeight(Mathf.RoundToInt(y_01 * terrainData.heightmapHeight), Mathf.RoundToInt(x_01 * terrainData.heightmapWidth));
                Vector3 normal = terrainData.GetInterpolatedNormal(y_01, x_01);

                float steepness = terrainData.GetSteepness(y_01, x_01);

                // Setup an array to record the mix of texture weights at this point
                float[] splatWeights = new float[terrainData.alphamapLayers];

                // Texture[0] has constant influence
                splatWeights[0] = 0.1f;

                // Texture[1] is stronger at lower altitudes
                splatWeights[1] = Mathf.Clamp01(height);


                // Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
                float z = splatWeights.Sum();

                // Loop through each terrain texture
                for (int i = 0; i < terrainData.alphamapLayers; i++)
                {
                    // Normalize so that sum of all texture weights = 1
                    splatWeights[i] /= z;

                    // Assign this point to the splatmap array
                    splatmapData[x, y, i] = splatWeights[i];
                }
            }
        }

        // Finally assign the new splatmap to the terrainData:
        terrainData.SetAlphamaps(0, 0, splatmapData);


        return terrainData;
    }

	float[,] InitHeights ()
    {	
		Vector2 [] centers = new Vector2[]{new Vector2(250, 250), new Vector2(200, 200), new Vector2(300, 300)};
		float [] mags = new float[]{10f, 20f, 30f};
        //initialize heights to 0 across terrain
        float[,] heights = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
				heights[x,y] = 0;
				for (int c = 0; c < centers.Length ; c++)
				{
					if (Vector2.Distance(centers[c], new Vector2(x,y)) < 30f) { // 
						float exp = -( (x - centers[c].x)*(x - centers[c].x) + (y - centers[c].y)*(y - centers[c].y)) / (2f * sigma * sigma);
						float amp = a / (sigma*sigma * 2*Mathf.PI); 
						heights[x,y] = amp * Mathf.Exp(exp);
					}
				}
				
				
            }
         }
        return heights;
     }

	 float[,] SetMoundHeights ()
    {	
		
        //initialize heights to 0 across terrain
        float[,] heights = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
				// if x,y is near a center
				float exp = -( (x - 250f)*(x - 250f) + (y - 250f)*(y - 250f)) / (2f * sigma * sigma);
				float amp = a / (sigma*sigma * 2*Mathf.PI); 
				heights[x,y] = amp * Mathf.Exp(exp);
            }
         }
        return heights;
     }

	

}
