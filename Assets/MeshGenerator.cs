using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour {

	Mesh mesh;
	Vector3 [] vertices;
	int [] triangles;

	Vector2[] uvs;

	public float w = 5; // half width of strip
	public float R = 100; // inner circle

	public int steps = 100; // so 300 points

	// Use this for initialization
	void Start () {
		 
		mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		GetComponent<MeshCollider>().sharedMesh = mesh;


		CreateShape();
		UpdateMesh();

	}
	



	void CreateShape() 
	{

		vertices = new Vector3[steps * 3 * 2];
		
		float t = 0, s = 0, x=0, y=0, z=0;
		float tStep = 4f* Mathf.PI / steps;
		for (int i=0,j=0; j < steps; j++)
		{
			for (int u=0; u < 3; u++)
			{
				s = (u-1)*w;
				x = (R + s * Mathf.Cos(t/2f)) * Mathf.Cos(t/2f);
				y = (R + s * Mathf.Cos(t/2f)) * Mathf.Sin(t/2f);
				z = s * Mathf.Sin(t/2f) + 20f;
				vertices[i] = new Vector3(x, y, z);
				vertices[steps*3+i] = new Vector3(x, y, z);
				i++;
				
			}
			t+=tStep;
		}

		int triPerSide = 3*steps * 4;
		
		triangles = new int[3*steps * 4 * 2];
		
		for (int i = 0, index = 0; i < steps*3; i+=3) // step of the strip
		{
			triangles[index] = i; // 0,1,3
			triangles[index+1] = (i+1) % (vertices.Length);
			triangles[index+2] = (i+3) % (vertices.Length);

			triangles[index+3] = i + 1;
			triangles[index+4] = i + 2;
			triangles[index+5] = (i +4) % (vertices.Length);

			triangles[index+6] = (i +3) % (vertices.Length);
			triangles[index+7] = i + 1;
			triangles[index+8] = (i +4) % (vertices.Length);

			triangles[index+9] = (i +4) % (vertices.Length);
			triangles[index+10] = i + 2;
			triangles[index+11] = (i +5) % (vertices.Length);


			triangles[triPerSide + index] = i; // 0,1,3
			triangles[triPerSide + index+1] = (i+3) % (vertices.Length);
			triangles[triPerSide + index+2] = (i+1) % (vertices.Length);

			triangles[triPerSide + index+3] = i + 2;
			triangles[triPerSide + index+4] = i + 1;
			triangles[triPerSide + index+5] = (i +4) % (vertices.Length);

			triangles[triPerSide + index+6] = (i +3) % (vertices.Length);
			triangles[triPerSide + index+7] = i + 4;
			triangles[triPerSide + index+8] = (i +1) % (vertices.Length);

			triangles[triPerSide + index+9] = (i +4) % (vertices.Length);
			triangles[triPerSide + index+10] = i + 5;
			triangles[triPerSide + index+11] = (i +2) % (vertices.Length);

			index +=12;

		}

	}

	void UpdateMesh () 
	{
		mesh.Clear();

		mesh.vertices = vertices;
		mesh.triangles = triangles;

		mesh.RecalculateNormals();

	}

	private void OnDrawGizmos ()
	{
		if (vertices == null) return;

		for (int i=0; i < vertices.Length; i++)
		{
			//Debug.Log("we here" + vertices[i]);
			 //UnityEditor.Handles.Label (vertices[i], i+"");

			//Gizmos.DrawSphere(vertices[i], 1f);
		}

		for (int i=0; i < mesh.vertexCount; i++)
		{
			//Gizmos.DrawLine(mesh.vertices[i], mesh.vertices[i] + mesh.normals[i]);
		}
	}

	 
}
