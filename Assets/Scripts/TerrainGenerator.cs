
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class TerrainGenerator : MonoBehaviour {
    //public int mapZ = 3000;
    public int mapX = 1000;
    public int mapY = 1000;
    public int width = 1000;
    public int height = 1000;
    public int border = 30;
    public float mazeWallHeight = 0.5f;
    public int depth = 20;
    public GameObject bulletPrefab;
    public GameObject enemyPrefab;
    public GameObject wallPrefab;
    public float scale = 20f;

    public float outerWallHeight = 10.0f;
    public float outerWallDepth = 5.0f;
    public float floorDepth = 5.0f;

    public int nx;
    public int ny;

    private int pillarHeight = 10;
    private int pillarWidth = 50;

    // should be a square number 
    public int n = 5;

    public List<GameObject> bullets;

    private Dictionary<int, List<int>> G;
    private Dictionary<int, List<int>> F;
    private Dictionary<int, Vector2> centers;
    private Dictionary<int, bool> marks;

    static  System.Random rnd = new System.Random();

    private void Start()
    {


        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
            
        bullets = new List<GameObject>();
        
        GenerateOuterWalls();
        

    }

    private void GenerateOuterWalls ()
    {
        // front wall
        var wall0 = (GameObject)Instantiate(wallPrefab, new Vector3(width/2.0f + 1.0f*width/n - 5.0f, outerWallHeight/2.0f, 0), new Quaternion(0,0,0,0));
        wall0.transform.localScale = new Vector3(width, outerWallHeight, outerWallDepth);

        // back wall
        var wall3 = (GameObject)Instantiate(wallPrefab, new Vector3(width/2.0f, outerWallHeight/2.0f, width), new Quaternion(0,0,0,0));
        wall3.transform.localScale = new Vector3(width, outerWallHeight, outerWallDepth);

        // side wall1
        var wall1 = (GameObject)Instantiate(wallPrefab, new Vector3(0, outerWallHeight/2.0f, width/2.0f), new Quaternion(0,0,0,0));
        wall1.transform.localScale = new Vector3(outerWallDepth, outerWallHeight, width);

        // side wall2
        var wall2 = (GameObject)Instantiate(wallPrefab, new Vector3(width, outerWallHeight/2.0f, width/2.0f), new Quaternion(0,0,0,0));
        wall2.transform.localScale = new Vector3(outerWallDepth, outerWallHeight, width);

        

        // ceiling
        var floor1 = (GameObject)Instantiate(wallPrefab, new Vector3(width/2.0f, outerWallHeight, width/2.0f), new Quaternion(0,0,0,0));
        floor1.transform.localScale = new Vector3(width, floorDepth, width);


    }

    void GenerateEdges(TerrainData terrainData) 
    {

         G[0].Clear();

        foreach (var v in G)
        {
            Debug.Log("vertex v has position" + vertexLocation(v.Key));

            Vector2 currLoc = vertexLocation(v.Key);
            foreach (var neighbor in v.Value)
            {
                Vector2 neighborLoc = vertexLocation(neighbor);
                if (currLoc.x < neighborLoc.x || currLoc.y < neighborLoc.y)
                {
                    terrainData.SetHeights((int)currLoc.x, (int)currLoc.y, PillarHeights(currLoc, neighborLoc));

                }

            }

        }
    }

    TerrainData GenerateTerrain (TerrainData terrainData)
    {
        terrainData.size = new Vector3(width, depth, height);

        // initialize all heights to 0
        terrainData.SetHeights(0, 0, InitHeights());

        GenerateMazeGraphs();

        // the first wall as a door
        G[0].Remove(1);

        GenerateEdges(terrainData);
       



        // Splatmap data is stored internally as a 3d array of floats, so declare a new empty array ready for your custom splatmap data:
        float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                // Normalise x/y coordinates to range 0-1 
                float y_01 = (float)y / (float)terrainData.alphamapHeight;
                float x_01 = (float)x / (float)terrainData.alphamapWidth;

                // Sample the height at this location (note GetHeight expects int coordinates corresponding to locations in the heightmap array)
                float height = terrainData.GetHeight(Mathf.RoundToInt(y_01 * terrainData.heightmapHeight), Mathf.RoundToInt(x_01 * terrainData.heightmapWidth));

                // Calculate the normal of the terrain (note this is in normalised coordinates relative to the overall terrain dimensions)
                Vector3 normal = terrainData.GetInterpolatedNormal(y_01, x_01);

                // Calculate the steepness of the terrain
                float steepness = terrainData.GetSteepness(y_01, x_01);

                // Setup an array to record the mix of texture weights at this point
                float[] splatWeights = new float[terrainData.alphamapLayers];

                // CHANGE THE RULES BELOW TO SET THE WEIGHTS OF EACH TEXTURE ON WHATEVER RULES YOU WANT

                // Texture[0] has constant influence
                splatWeights[0] = 0.1f;

                // Texture[1] is stronger at lower altitudes
                splatWeights[1] = Mathf.Clamp01(height);

                // Texture[2] stronger on flatter terrain
                // Note "steepness" is unbounded, so we "normalise" it by dividing by the extent of heightmap height and scale factor
                // Subtract result from 1.0 to give greater weighting to flat surfaces
                splatWeights[2] = Mathf.Clamp01(steepness * steepness / (terrainData.heightmapHeight / 5.0f));

                // Texture[3] increases with height but only on surfaces facing positive Z axis 
                splatWeights[3] = height * Mathf.Clamp01(normal.z);

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

    Vector2 vertexLocation(int indexG) {
        float step = width / n;
        int row = (int)Math.Floor(1f*indexG / (n+1));
        return new Vector2(step * (indexG % (n+1)), row * step);
    }

    Dictionary<int, List<int>> CreateConnectedGrid( int s) 
    {
        //build a grid where cells point to each other
        Dictionary<int, List<int>> g = new Dictionary<int, List<int>>();

        for (int i = 0; i < s * s; i++)
        {
            List<int> neighbors = new List<int>();
            // add cell below if not in bottom row

            if (i < s * s- s)
            {
                neighbors.Add(i + s);
            }
            // add cell to right if not in last column
            if (i % s != s - 1)
            {
                neighbors.Add(i + 1);
            }
            // add cell to left if not in first column
            if (i % s != 0)
            {
                neighbors.Add(i - 1);
            }
            // add cell above if not on first row
            if (i >= s)
            {
                neighbors.Add(i - s);
            }
            g.Add(i, neighbors);

        }
        return g;
    }

    Dictionary<int, Vector2> GetCenters()
    {

        //build a grid where cells point to each other
        Dictionary<int, Vector2> result = new Dictionary<int, Vector2>();


        float step = 1f*width / n;
        float currX = step / 2f;
        float currY = step / 2f;
        foreach (var cell in F) {

            result.Add(cell.Key, new Vector2(currY, currX));


            // create a bullet marker at this spot
            Vector3 pos = new Vector3(currX, 1, currY);
            Quaternion rot = new Quaternion();
            if (cell.Key == 3 || cell.Key == n*n-1) {
                var bullet = (GameObject)Instantiate(bulletPrefab, pos, rot);
                bullet.transform.localScale = new Vector3(5, 5, 5);
            }

            if (cell.Key == 10) {
                var enemy = (GameObject)Instantiate(enemyPrefab, pos, rot);

            }


            currX += step;
            if (currX > width) {
                currX = step / 2f;
                currY += step;
            }
        }

        return result;
    }

    void backtrack() {
        Stack<int>cellStack = new Stack<int>();
        marks = new Dictionary<int, bool>();
        foreach (var cell in F)
        {
            marks[cell.Key] = false;
        }


        int curr = 0;
        marks[curr] = true;

        while(someCellUnmarked(marks)) {

            List<int> unvisitedNeighbors = GetUnvisitedNeighbors(curr);
            if(unvisitedNeighbors.Count > 0) {
                int randomIndex = rnd.Next(unvisitedNeighbors.Count);
                cellStack.Push(curr);
                int chosen = unvisitedNeighbors[randomIndex];

                //remove corresponding edge in G
                int vBR = getBottomRightInG(curr);
                int vBL = vBR - 1;
                int vTR = vBR + n + 1;
                int vTL = vBL + n + 1;

                //Debug.Log("Removing Edge " + curr + "-" + chosen);

                if (curr - chosen == -1) {
                    // to the right, 
                    G[vBR].Remove(vTR);
                    G[vTR].Remove(vBR);
                }

                if (curr - chosen == 1)
                {
                    // to the left, 
                    G[vBL].Remove(vTL);
                    G[vTL].Remove(vBL);
                }

                if (curr - chosen == n) {
                    // to the below
                    G[vBL].Remove(vBR);
                    G[vBR].Remove(vBL);
                }

                if (curr - chosen == -n)
                {
                    // to the above
                    G[vTL].Remove(vTR);
                    G[vTR].Remove(vTL);
                }





                curr = chosen;
                marks[curr] = true;
            } else if (cellStack.Count > 0) {
                int poppedCell = cellStack.Pop();
                curr = poppedCell;
            }
        }


    }

    List<int> GetUnvisitedNeighbors(int index) {
        List<int> unvisited = new List<int>();
        foreach(var neighbor in F[index]) {
            if (!marks[neighbor]) {
                unvisited.Add(neighbor);
            }
        }
        return unvisited;
    }

    bool someCellUnmarked(Dictionary<int,bool> marks) 
    {
        bool result = false;
        foreach (var cell in marks)
        {
            if (!cell.Value) {
               result = true;
            }
        }
        return result;
    }

    void GenerateMazeGraphs ()
    {
        // edge map is G
        G = CreateConnectedGrid(n + 1);
        // cell map is F
        F = CreateConnectedGrid(n);

        // step 2 calculate the centers
        centers = GetCenters();

        // step 3 remove edges from G
        backtrack();

        /*
         // print info regarding G like this
        int[] ints = G[18].ToArray();
        string[] result = Array.ConvertAll(ints, x => x.ToString());
        print("G [18] (edge graph) " + string.Join(",", result));
        */
    }



    float[,] InitHeights ()
    {
        
        //initialize heights to 0 across terrain
        float[,] heights = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = 0;
            }
         }
        return heights;
     }

    float[,] PillarHeights(Vector2 loc1, Vector2 loc2)
    {
        int numX = pillarHeight;
        int numY = pillarWidth;

        if(loc1.x != loc2.x) {
            numX = pillarWidth;
            numY = pillarHeight;
        }




        //step X set height of each x,y coordinate in the terrain
        float[,] heights = new float[numY, numX];


        for (int x = 0; x < numX; x++)
        {
            for (int y = 0; y < numY; y++)
            {
                heights[y, x] = mazeWallHeight;
            }
        }
        return heights;
    }

    int getRowForX(float inputX)
    {
        float step = 1f*width / n;
        float limit = step;
        int row = -1;
        for (int i = 0; i < n && row==-1 ; i++)
        {
            if (inputX < limit ){
                row = i;
            }
            limit += step;
        }
        if(row==-1) {
            row = n-1;
        }
        return row;

    }

    int getColForY(float inputY)
    {
        float step = 1f * width / n;
        float limit = step;
        int col = -1;
        for (int i = 0; i < n && col == -1; i++)
        {
            if (inputY < limit)
            {
                col = i;
            }
            limit +=step;
        }
        if (col == -1)
        {
            col = n - 1;
        }
        return col;

    }

    int getFIndex(int row, int col) {

        return row*n+col;
    }

    int getBottomRightInG(int fIndex) {
        return fIndex + 1 + (int)Math.Floor(1f * fIndex / n);
    }

    int getTopLeftInG(int fIndex)
    {
        return fIndex  + (int)Math.Floor(1f * fIndex / n);
    }


}

