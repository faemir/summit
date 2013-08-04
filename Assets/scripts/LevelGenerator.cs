﻿/* Level Generator
 * This script generates a roughly cylindrical climbing wall
 */

using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour 
{
	public Material material;			// The material to use on the mesh renderer
	public float layerHeight = 1f;		// The Y distance between circles
	public float startRadius = 10f;		// The radius of the first 'circle'
	public int layers = 32;				// The number of circles to draw
	public int sides = 128;				// The number of sides for this 'circle'
	public float maxLean = 1f;			// The maximum X/Y distance between each circle
	public float maxPinch = 1f;			// The maximum change in radius between circles
	public bool debug = false;
	
	private float radius;
	private float innerAngle = 0f;
	private Vector3 columnHead = Vector3.zero;
	// Mesh related data
	private Vector3[] vertices;
	private Vector2[] uv;
	private int[] triangles;
	
	// Nodes
	private int nodeIndex = -1;			// The current 'side' that a node is on
	private int nodeMaxVariance = 1; 	//maximum distance between nodes (on X/Y plane) in vertices
	
	
	void Start () 
	{
		// Initial values
		radius = startRadius;
		innerAngle = 360f / (float)sides;
		nodeIndex = Random.Range (0,sides);
		// Start generation
		Generate();
	}

	void Generate()
	{
		int side = 0;
		int layer = 0;
		
		gameObject.AddComponent<MeshFilter>();
		gameObject.AddComponent<MeshRenderer>();
		renderer.material = material;
		
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		vertices = new Vector3[layers * sides];
		uv = new Vector2[layers * sides];
		triangles = new int[layers * sides * 6];
		
		// Create vertices for each layer 
		for ( layer = 0; layer < layers; layer++ )
		{
			// Create a vertex for each 'side', then rotate by innerAngle
			for ( side = 0; side < sides; side++ )
			{
				Vector3 vertex = columnHead + (transform.forward * radius);
				//node check
				if (side == nodeIndex) {
					vertex = columnHead;
					Debug.DrawLine(columnHead, vertex, Color.yellow, Mathf.Infinity);
				}
				vertices[ layer*sides + side ] = vertex;
				uv[layer*sides + side] = new Vector2(side, layer);
				if (debug)
					Debug.DrawLine(columnHead, vertex, Color.magenta, Mathf.Infinity);
				
				transform.Rotate(Vector3.up, innerAngle);
			}
			// Before moving onto the next layer move the columnHead up
			Vector3 nextColumnHead = new Vector3(Random.Range(-maxLean,maxLean), layerHeight, Random.Range(-maxLean, maxLean));
			if (debug)
				Debug.DrawLine(columnHead, columnHead + nextColumnHead, Color.cyan, Mathf.Infinity);
			columnHead += nextColumnHead;
			// and modify the radius
			radius += Random.Range(-maxPinch, maxPinch);
			//set the next nodeIndex, code is placed here assuming one node per layer
			nodeIndex = Random.Range (-nodeMaxVariance,nodeMaxVariance) % sides;
		}
		
		// For simplicity, fill the triangles index after creating all the vertices.
		// Each triangles[] element is an index to the vertices[] array
		// So each element in triangles[] is really indicating a point on a triangle
		// Every 3 points are the points for one triangle.
		int index = 0;
		for ( layer =0; layer < layers-1; layer++ )
		{
			for ( side = 0; side < sides-1; side++ )
			{
				// Two triangles make a square.
				// Triangle one
				triangles[index++] = (layer    *sides) + side;
				triangles[index++] = (layer    *sides) + side + 1;
				triangles[index++] = ((layer+1)*sides) + side;
				// Triangle two
				triangles[index++] = ((layer+1)*sides) + side;
				triangles[index++] = ( layer   *sides) + side + 1;
				triangles[index++] = ((layer+1)*sides) + side + 1;
			}
			// These two triangles join the ends of each layer
			// This is best explained by removing the code for them to see what happens...
			// (hint: look all around the mesh, easier to spot with less sides)
			triangles[index++] = ( layer   *sides);
			triangles[index++] = ((layer+1)*sides);
			triangles[index++] = ( layer   *sides) + sides-1;
			
			triangles[index++] = ((layer+1)*sides);
			triangles[index++] = ((layer+1)*sides) + sides-1;
			triangles[index++] = ( layer   *sides) + sides-1;
		}
		// Assign our mesh data to the mesh
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
		// Create a physics component for our new mesh (mesh ignores all collisions otherwise)
		gameObject.AddComponent<MeshCollider>();
	}
}