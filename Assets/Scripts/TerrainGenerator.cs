
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class TerrainGenerator : MonoBehaviour {
    public int n = 3;
    public int startCellIndex = 4;
    private int endingPoint = 0;
    public int width = 500;
    public int height = 500;
    public int depth = 20;
    
    public GameObject bulletPrefab;
    public GameObject enemyPrefab;
    public GameObject wallPrefab;
    public GameObject facadePrefab;
    public GameObject rampPrefab;

    public GameObject terrainSectionPrefab;

    public Texture holeTexture;
    public Texture floorTexture;


    public float outerWallHeight = 100.0f;
    public float outerWallDepth = 5.0f;
    public float floorDepth = 5.0f;

    public float edgeLength;
    public float edgeWidth;
    

    public List<GameObject> bullets;

    private Dictionary<int, List<int>> G;
    private Dictionary<int, List<int>> ZG;
    private Dictionary<int, List<int>> F;
    private Dictionary<int, Vector3> centers;
    private Dictionary<int, bool> marks;

    static  System.Random rnd = new System.Random();
  

    private void Start()
    {

        edgeLength = 1.0f*width/n;
        edgeWidth = 10.0f;

        //  GenerateRamp();
       Terrain terrain = GetComponent<Terrain>();
       // Debug.Log(terrain.terrainData.size);
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
            
        bullets = new List<GameObject>();


        GenerateHoles();
        createFinishPoint();
       // GenerateTerrains();
        //GenerateOuterWalls();
        

    }

    private void createFinishPoint() {
        var endPoint = (GameObject)Instantiate(bulletPrefab, 
            new Vector3(centers[endingPoint].x, centers[endingPoint].z, centers[endingPoint].y), 
            new Quaternion());
            endPoint.gameObject.name = "Finish Point";
            endPoint.transform.localScale = new Vector3(5, 5, 5);
                
                
            
    }

    private void GenerateTerrains () {
        
        GameObject terrainObject = new GameObject("TerrainObj");
        terrainObject.transform.position = new Vector3(-500,0,-500);
        TerrainData td = new TerrainData();
        td.size = new Vector3(width, depth, width);
        SplatPrototype[] terrainTexture = new SplatPrototype[1];
        terrainTexture[0] = new SplatPrototype();
        terrainTexture[0].texture = (Texture2D)Resources.Load("TerrainTextures/orange");
        td.splatPrototypes = terrainTexture;
        TerrainCollider _TerrainCollider = terrainObject.AddComponent<TerrainCollider>();
        Terrain terrainComponent = terrainObject.AddComponent<Terrain>();

        // initialize all heights to 0
        td.SetHeights(0, 0, TestHeights());

        _TerrainCollider.terrainData = td;
        terrainComponent.terrainData = td;
            
    }


  
//   private void GenerateRamp() {
//       // front ramp
//         var ramp = (GameObject)Instantiate(rampPrefab, new Vector3(width/2.0f, 0, -outerWallHeight), new Quaternion(0,0,0,0));
//         ramp.gameObject.name = "Ramp - Front";
//         ramp.transform.localScale = new Vector3(3.0f, outerWallHeight/2.0f, outerWallHeight/2.0f);
//   }

  private void GenerateHoles()
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.gameObject.name = "Hole1";
        sphere.gameObject.transform.position = new Vector3(250, 0, 0);
        sphere.gameObject.transform.localScale = new Vector3(10, 10, 1);
        sphere.gameObject.layer = 8;
        sphere.gameObject.AddComponent<Rigidbody>();
        sphere.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        sphere.gameObject.GetComponent<Collider>().isTrigger = true;
        sphere.gameObject.GetComponent<Rigidbody>().useGravity = false;
        sphere.gameObject.GetComponent<Rigidbody>().gameObject.transform.localScale = new Vector3(10, 10, 1);
        sphere.gameObject.GetComponent<Renderer>().material.mainTexture = holeTexture;

 
        var sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.radius = 1.5f;
        sphereCollider.isTrigger = true;
    }
    

     

    private void GenerateOuterWalls ()
    {
        // front wall
        var wall0 = (GameObject)Instantiate(wallPrefab, new Vector3(width/2.0f + 1.0f*width/n - 5.0f, outerWallHeight/2.0f, 0), new Quaternion(0,0,0,0));
        wall0.gameObject.name = "Outer Wall - Front";
        wall0.transform.localScale = new Vector3(width, outerWallHeight, outerWallDepth);

        // back wall
        var wall3 = (GameObject)Instantiate(wallPrefab, new Vector3(width/2.0f, outerWallHeight/2.0f, width), new Quaternion(0,0,0,0));
        wall3.gameObject.name = "Outer Wall - Back";
        wall3.transform.localScale = new Vector3(width, outerWallHeight, outerWallDepth);

        // side wall1
        var wall1 = (GameObject)Instantiate(wallPrefab, new Vector3(0, outerWallHeight/2.0f, width/2.0f), new Quaternion(0,0,0,0));
        wall1.gameObject.name = "Outer Wall - Right";
        wall1.transform.localScale = new Vector3(outerWallDepth, outerWallHeight, width);

        // side wall2
        var wall2 = (GameObject)Instantiate(wallPrefab, new Vector3(width, outerWallHeight/2.0f, width/2.0f), new Quaternion(0,0,0,0));
        wall2.gameObject.name = "Outer Wall - Left";
        wall2.transform.localScale = new Vector3(outerWallDepth, outerWallHeight, width);

    }

    // void GenerateEdges(TerrainData terrainData) 
    // {
    //     foreach (var v in G)
    //     {
    //         Vector2 currLoc = vertexLocation(v.Key);
    //         foreach (var neighbor in v.Value)
    //         {   


    //             Vector2 neighborLoc = vertexLocation(neighbor);

    //             bool onEdge = (v.Key < n && neighbor < n) || // front edge
    //                           (v.Key % (n+1) == 0 && neighbor %(n+1) == 0) ||  // left edge 
    //                           ((v.Key + 1 )% (n+1) == 0 && (neighbor +1) %(n+1) == 0) ;
    //             if ((currLoc.x < neighborLoc.x || currLoc.y < neighborLoc.y) && !onEdge)
    //             {
    //                 Quaternion rot = new Quaternion(0,0,0,0);
    //                 Vector3 scale = new Vector3(edgeLength, edgeWidth, edgeWidth);
    //                 Vector3 pos = new Vector3(currLoc.x + edgeLength/2.0f,edgeWidth/2.0f, currLoc.y);
    //                 if((int)Math.Abs(v.Key - neighbor) == n+1) {
    //                     // vertical wall has different position / scale
    //                     scale = new Vector3(edgeWidth, edgeWidth, edgeLength);
    //                     pos = new Vector3(currLoc.x, edgeWidth/2.0f, currLoc.y+ edgeLength/2.0f);
    //                 } 
    //                 var w = (GameObject)Instantiate(wallPrefab, pos, rot);
    //                 w.gameObject.name = "Maze Wall - " + v.Key + "-" +neighbor;
    //                 w.transform.localScale = scale;
    //             }

    //         }

    //     }
    // }

    void GenerateEdges2() 
    {
        foreach (var v in G)
        {
            Vector3 currLoc = vertexLocationInG(v.Key);
            foreach (var neighbor in v.Value)
            {   


                Vector3 neighborLoc = vertexLocationInG(neighbor);

                var w = (GameObject)Instantiate(wallPrefab, (currLoc + neighborLoc) / 2f, new Quaternion(0,0,0,0));

                float wallLength =1f* width / n;
                 w.transform.localScale = new Vector3(wallLength, wallLength, 1f);;

                 w.gameObject.name = "Wall-" + v.Key + "-" +neighbor;

                 if (neighbor - v.Key  == ((n+1)*(n+1) + n+1))
                 {      

                     w.transform.localRotation = Quaternion.Euler(0,90,0);
                 }


                 if (neighbor - v.Key  == n+2)
                 {  
                     w.gameObject.GetComponent<Renderer>().material.mainTexture = floorTexture;    
                     w.transform.localRotation = Quaternion.Euler(90,0,0);
                 }

            }

        }
    }

    TerrainData GenerateTerrain (TerrainData terrainData)
    {
        terrainData.size= new Vector3(width, depth, height);

        // initialize all heights to 0
        terrainData.SetHeights(0, 0, InitHeights());

        GenerateMazeGraphs();
       
        

        
 
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

    

    Dictionary<int, List<int>> CreateConnectedBox( int s) 
    {
        //build a grid where cells point to each other
        Dictionary<int, List<int>> g = new Dictionary<int, List<int>>();
        int s2 = s*s;
        int s3 = s*s*s;
        for (int i = 0; i < s * s * s; i++)
        {
            List<int> neighbors = new List<int>();
            // add cell below if not in bottom row
            if ((i%s2) < s2- s)
            {
                neighbors.Add(i + s);
            }
            // add cell to right if not in last column
            if (i% s != s - 1)
            {
                neighbors.Add(i + 1);
            }
            // add cell to left if not in first column
            if (i % s != 0)
            {
                neighbors.Add(i - 1);
            }
            // add cell above if not on first row
            if ((i%s2) >= s)
            {
                neighbors.Add(i - s);
            }

            if (i<s3-s2) {
                neighbors.Add(i + s2);
            }
            if (i>=s2) {
                neighbors.Add(i - s2);
            }
            g.Add(i, neighbors);

        }
        return g;
    }

    Dictionary<int, List<int>> CreateConnectedBox2( int s) 
    {
        //build a grid where cells point to each other
        Dictionary<int, List<int>> g = new Dictionary<int, List<int>>();
        int s2 = s*s;
        int s3 = s*s*s;
        for (int i = 0; i < s * s * s; i++)
        {
            List<int> neighbors = new List<int>();
            

            if (i+1<s3-s2 && ((i+1) % (n+1)) != 0) {
                // up and to the right
                neighbors.Add(i + s2 + 1);
            }

             if (i+n+1+(n+1)*(n+1)<s3 && (i % ((n+1)*(n+1))) < (n+1)*(n+1) - (n+1)) {
                // up and to the front
                neighbors.Add(i + n+1 +(n+1)*(n+1));
            }

            if (((i+1) % (n+1) != 0) && (i % ((n+1)*(n+1))) < (n+1)*(n+1) - (n+1)) {
                // same floor, diagonal forwards
                 neighbors.Add(i + n + 2);
            }
            
            g.Add(i, neighbors);

        }
        return g;
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

    Dictionary<int, Vector3> GetCenters2()
    {

        //build a grid where cells point to each other
        Dictionary<int, Vector3> result = new Dictionary<int, Vector3>();
            
        Debug.Log("here and getting centers");
        float step = 1f*width / n;
        float currX = step / 2f;
        float currY = step / 2f;
        float currZ = step / 2f;
        foreach (var cell in F) {
            Debug.Log("key" + cell.Key);
            result.Add(cell.Key, new Vector3(currX, currZ, currY));


            // create a bullet marker at this spot
            Vector3 pos = new Vector3(currX, currZ, currY);
            Quaternion rot = new Quaternion();
            if (cell.Key == startCellIndex ) {
                var bullet = (GameObject)Instantiate(bulletPrefab, pos, rot);
                bullet.gameObject.name = "Starting Point";
                bullet.transform.localScale = new Vector3(3, 3, 3);
            }

            if (cell.Key == n*n-1) {
                
            }

            if (cell.Key == 10) {
                var enemy = (GameObject)Instantiate(enemyPrefab, pos, rot);

            }


            currX += step;
            if (currX > width) {
                currX = step / 2f;
                currY += step;
            }
            if (currY > width) {
                currY = step / 2f;
                currZ += step;
            }
        }

        return result;
    }
    
    // Dictionary<int, Vector2> GetCenters()
    // {

    //     //build a grid where cells point to each other
    //     Dictionary<int, Vector2> result = new Dictionary<int, Vector2>();


    //     float step = 1f*width / n;
    //     float currX = step / 2f;
    //     float currY = step / 2f;
    //     foreach (var cell in F) {

    //         result.Add(cell.Key, new Vector2(currY, currX));


    //         // create a bullet marker at this spot
    //         Vector3 pos = new Vector3(currX, 1, currY);
    //         Quaternion rot = new Quaternion();
    //         if (cell.Key == startCellIndex ) {
    //             var bullet = (GameObject)Instantiate(bulletPrefab, pos, rot);
    //             bullet.gameObject.name = "Starting Point";
    //             bullet.transform.localScale = new Vector3(3, 3, 3);
    //         }

    //         if (cell.Key == n*n-1) {
                
    //         }

    //         if (cell.Key == 10) {
    //             var enemy = (GameObject)Instantiate(enemyPrefab, pos, rot);

    //         }


    //         currX += step;
    //         if (currX > width) {
    //             currX = step / 2f;
    //             currY += step;
    //         }
    //     }

    //     return result;
    // }
    
    void backtrack() {
        Stack<int>cellStack = new Stack<int>();
        marks = new Dictionary<int, bool>();
        foreach (var cell in F)
        {
            marks[cell.Key] = false;
        }

        int curr = startCellIndex;
        marks[curr] = true;

        while(someCellUnmarked(marks)) {

            List<int> unvisitedNeighbors = GetUnvisitedNeighbors(curr);
            if(unvisitedNeighbors.Count > 0) {
                cellStack.Push(curr);
                int randomIndex = rnd.Next(unvisitedNeighbors.Count);
                int randomIndex2 = rnd.Next(unvisitedNeighbors.Count);

                
                
                int chosen = unvisitedNeighbors[randomIndex];
                int n2 = n*n;

                if (unvisitedNeighbors[randomIndex2] < unvisitedNeighbors[randomIndex2]) {
                    chosen = unvisitedNeighbors[randomIndex2];
                }

                //remove corresponding edge in G
                // int vBR = getBottomRightInG(curr);
                // int vBL = vBR - 1;
                // int vTR = vBR + n + 1;
                // int vTL = vBL + n + 1;

                // lowest vertex
                int v0 = getLowestVertexInG(curr);
                int v1 = v0 + 1;
                int v2 = v0 + n + 2;
                int v3 = v0 + n + 1;
                int v4 = v0 + (n+1)*(n+1);
                int v5 = v0 + 1 + (n+1)*(n+1);
                int v6 = v0 + n + 2 + (n+1)*(n+1);
                int v7 = v0 + n + 1 + (n+1)*(n+1);


                Debug.Log("Removing Edge " + curr + "-" + chosen);

                // moved forward (one index +1)
                if (curr - chosen == -1) {
                    // to the right, 
                    G[v1].Remove(v6);
                }

                if (curr - chosen == 1)
                {
                    // to the left, 
                    G[v0].Remove(v7);
 
                }

                if (curr - chosen == n) {
                    // to the back
                    G[v0].Remove(v5);
                }

                if (curr - chosen == -n)
                {
                    // to the front
                    
                    G[v3].Remove(v6);
                }
                if (curr - chosen == n*n)
                {
                    G[v0].Remove(v2);
                }
                if (curr - chosen == -n*n)
                {
                    G[v4].Remove(v6);
                }

                curr = chosen;
                marks[curr] = true;
            } else if (cellStack.Count > 0) {
                int poppedCell = cellStack.Pop();
                curr = poppedCell;
            }
        }
        endingPoint = curr;

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
        //G = CreateConnectedGrid(n + 1);
        G = CreateConnectedBox2(n + 1);
        // cell map is F
        F = CreateConnectedBox(n);

        // step 2 calculate the centers
        centers = GetCenters2();

        // step 3 remove edges from G
        backtrack();

        // remove middle front wall as door
        int doorIndex = getLowestVertexInG(startCellIndex);
        G[doorIndex].Remove(doorIndex+1 + (n+1)*(n+1));

        // turn on maze walls
        GenerateEdges2();

        


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

     float[,] TestHeights ()
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

    // int getFIndex(int row, int col) {

    //     return row*n+col;
    // }

    // int getBottomRightInG(int fIndex) {
    //     return fIndex+ 1 + (int)Math.Floor(1f * fIndex / n);
    // }

    int getLowestVertexInG(int fIndex) {
        int n2 = n*n;
        int col = fIndex %n;
        int row = (int)Math.Floor(1f * fIndex / n) % n;

        return col+ row * (n+1) + (int)Math.Floor(1f * fIndex / n2)*(n+1)*(n+1);
    }

    // int getTopLeftInG(int fIndex)
    // {
    //     return fIndex  + (int)Math.Floor(1f * fIndex / n);
    // }

    Vector2 vertexLocation(int indexG) {
        float step = width / n;
        int row = (int)Math.Floor(1f*indexG / (n+1));
        return new Vector2(step * (indexG % (n+1)), row * step);
    }

    Vector3 vertexLocationInG(int i) {
        float step = 1f * width / n;
        int row = (int) Math.Floor(1f*(i % ((n+1) * (n+1))) / (n+1));
        int col = i % (n+1);
        int floor = (int) Math.Floor(1f*i / ((n+1)*(n+1)));
        //Debug.Log(i + " " + row + "," + )
        return new Vector3(col*step,floor*step - width, row*step);
    }

    void OnDrawGizmos()
    {
        foreach (var v in G)
        {
            Vector3 currLoc = vertexLocationInG(v.Key);

            //Handles.Label(currLoc, v.Key+"");
            foreach (var neighbor in v.Value)
            {   
                Vector3 neighborLoc = vertexLocationInG(neighbor);
                Gizmos.DrawLine(currLoc, neighborLoc);

            }

        }

        foreach (var v in centers)
        {

            Handles.Label(centers[v.Key], v.Key+"");
            

        }
    
    }

}

