using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zig : MonoBehaviour {
	Vector3[] newVertices;
	public float width =  500;
	public float h = 200.0f;
  Vector3[] newNormals;
    Vector2[] newUV;
    int[] newTriangles;
	// Use this for initialization
	void Start () {
		/* 
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        newVertices = new Vector3[8];

        newVertices[0] = new Vector3(0, 0, 0);
        newVertices[1] = new Vector3(width, 0, 0);
        newVertices[2] = new Vector3(0, h, 0);
        newVertices[3] = new Vector3(width, h, 0);

		newVertices[4] = new Vector3(width, h, width);
        newVertices[5] = new Vector3(width, 0, width);
        newVertices[6] = new Vector3(0, h, width);
        newVertices[7] = new Vector3(0, 0, width);
        

        newTriangles = new int[24];
        //  Lower left triangle.
        newTriangles[0] = 0;
        newTriangles[1] = 2;
        newTriangles[2] = 1;
        //  Upper right triangle.   
        newTriangles[3] = 2;
        newTriangles[4] = 3;
        newTriangles[5] = 1;
		//right lower let
		newTriangles[6] = 1;
		newTriangles[7] = 3;
		newTriangles[8] = 5;
		// right upper
		newTriangles[9] = 3;
		newTriangles[10] = 4;
		newTriangles[11] = 5;

		//upper back
		newTriangles[12] = 4;
		newTriangles[13] = 6;
		newTriangles[14] = 5;
		// lower back
		newTriangles[15] = 5;
		newTriangles[16] = 6;
		newTriangles[17] = 7;

		//lower left
		newTriangles[18] = 6;
		newTriangles[19] = 0;
		newTriangles[20] = 7;
		// upper left
		newTriangles[21] = 6;
		newTriangles[22] = 2;
		newTriangles[23] = 0;

        newNormals = new Vector3[8];
        newNormals[0] = -Vector3.forward;
        newNormals[1] = -Vector3.forward;
        newNormals[2] = -Vector3.forward;
        newNormals[3] = -Vector3.forward;
		newNormals[4] = Vector3.forward;
        newNormals[5] = Vector3.forward;
        newNormals[6] = Vector3.forward;
        newNormals[7] = Vector3.forward;

        newUV = new Vector2[8];
        newUV[0] = new Vector2(0, 0);
        newUV[1] = new Vector2(1, 0);
        newUV[2] = new Vector2(0, 1);
        newUV[3] = new Vector2(1, 1);
		newUV[4] = new Vector2(0, 0);
        newUV[5] = new Vector2(1, 0);
        newUV[6] = new Vector2(0, 1);
        newUV[7] = new Vector2(1, 1);


        mesh.vertices = newVertices;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;
        mesh.normals = newNormals;
		*/
	}
	
	
	// Update is called once per frame
	void Update () {
		
	}
}
