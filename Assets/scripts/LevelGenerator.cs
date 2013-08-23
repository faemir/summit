/* This file is part of summit
 *
 * Copyright (C) 2013 Matthew Blickem <explosivose@gmail.com>
 *
 * This script is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * You should have received a copy of the GNU General Public
 * License along with this script; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA
 */

/* Level Generator
 * This script generates a roughly cylindrical climbing wall
 */

using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour 
{
	// Unique data for each stage (serializable)
	[System.Serializable]
	public class StageProperties
	{
		public Material[] materials;
		public Transform[] hazards;
		public int numberOfHazards;
		public int numberOfRoutes;
		public float height;
		public float maxSinAmp;
		public float minSinAmp;
		public float maxSinOff;
		public float minSinOff;
		public float maxSinFrq;
		public float minSinFrq;
		public float maxSinPhs;
		public float minSinPhs;
	}
	
	public Transform[] handHolds;
	public Transform cloudLayer;
	public int verticesBetweenSinusoids = 16;			// The number of sides for this 'circle'
	public int sinusoidCount = 4;
	public float layerHeight = 1f;
	public float vertVariation = 0.125f;
	public float minimumRadius = 2f;
	public StageProperties[] stages = new StageProperties[4];
	
	
	public bool debug = true;
	
	private int vertsPerLayer;
	private float innerAngle = 0f;
	private Vector3 layerCenter = Vector3.zero;
	private int trianglesIndex = 0;
	private Vector3[] verts;
	private Vector2[] uv;
	private int[] triangles;
	
	private struct layerInfo
	{
		public int index;
		public float[] sinValues;
		public layerInfo(int layerNumber, float[] sinusoidValues)
		{
			index = layerNumber;
			sinValues = sinusoidValues;
		}
	}
	
	void Start () 
	{

		
		// Start generation

		Generate();
		
	}
	
	void Generate()
	{
		// Initial values
		vertsPerLayer = verticesBetweenSinusoids * sinusoidCount;
		innerAngle = 360f / (float)vertsPerLayer;
		
		// Size mesh data
		int layerCount;
		int vertcount = 0;
		int trianglecount = 0;
		for ( int i = 0; i < stages.Length; i++ )
		{
			layerCount = (int)(stages[i].height / layerHeight);
			vertcount += (layerCount * vertsPerLayer);
			trianglecount += (layerCount * vertsPerLayer * 6);
		}
		verts = new Vector3[vertcount];
		uv = new Vector2[vertcount];
		triangles = new int[trianglecount];
		Debug.Log ("Number of vertices: " + vertcount);
		// Generate mesh data for each stage
		layerInfo info;
		info.index = 0;
		info.sinValues = new float[sinusoidCount];
		for ( int i = 0; i < sinusoidCount; i++)
		{
			info.sinValues[i] = Random.Range(stages[0].minSinOff, stages[0].maxSinOff);
		}

		for ( int i = 0; i < stages.Length; i++)
		{
			info = GenerateMeshData(stages[i], info);
		}
		// Create mesh
		CreateMesh();
	}
	
	layerInfo GenerateMeshData(StageProperties stage, layerInfo prevInfo)
	{
		int layerCount = (int)(stage.height / layerHeight);
		// <sinusoid parameters>
		float[] freq = new float[sinusoidCount];
		for ( int i = 0; i < sinusoidCount; i++)
		{
			freq[i] = Random.Range(stage.minSinFrq, stage.maxSinFrq);
		}
		
		float[] amp = new float[sinusoidCount];
		for ( int i = 0; i < sinusoidCount; i++)
		{
			amp[i] = Random.Range(stage.minSinAmp, stage.maxSinAmp);
		}
		
		float[] off = new float[sinusoidCount];
		for ( int i = 0; i < sinusoidCount; i++)
		{
			off[i] = Random.Range(stage.minSinOff, stage.maxSinOff);
		}
		
		float[] phas = new float[sinusoidCount];
		for ( int i = 0; i < sinusoidCount; i++)
		{
			phas[i] = Random.Range(stage.minSinPhs, stage.maxSinPhs);
		}
		// </sinusoid parameters>
		
		// <path nodes>
		int[] nodes = new int[stage.numberOfRoutes];
		for ( int i = 0; i < nodes.Length; i++)
		{
			nodes[i] = Random.Range(0, vertsPerLayer);
		}
		// </path nodes>
		
		// <hazards>
		int[] hazardVerts = new int[stage.numberOfHazards];
		for ( int i = 0; i < hazardVerts.Length; i++)
		{
			hazardVerts[i] = Random.Range(vertsPerLayer, layerCount*vertsPerLayer);
		}
		// </hazards>
		
		
		// <offset corrections>
		float[] sin = new float[sinusoidCount];
		for ( int i = 0; i < sinusoidCount; i++ )
		{
			sin[i] = amp[i] * Mathf.Sin(2f * Mathf.PI  * freq[i] * prevInfo.index + phas[i]) + off[i];
			off[i] += prevInfo.sinValues[i] - sin[i];
		}
		// </offset corrections>
		
		// <vertices and uv generation>
		float radius = 0f;
		float sinrad_next; float sinrad_prev;
		float sinvert_next; float sinvert_prev = 0f;
		for ( int layer = prevInfo.index; layer < prevInfo.index + layerCount; layer++ )
		{
			// calculate next sinusoid value
			for ( int i = 0; i < sinusoidCount; i++ )
			{
				sin[i] = Mathf.Abs(amp[i] * Mathf.Sin(2f * Mathf.PI  * freq[i] * layer + phas[i]) + off[i] - minimumRadius) + minimumRadius;
			}
			
			int sinIndex = 0;
			for ( int vert = 0; vert < vertsPerLayer; vert++ )
			{
				// insert sinusoid values as radii
				if ( vert % verticesBetweenSinusoids == 0 ) 
				{
					radius = sin[sinIndex++];
					sinvert_prev = vert;
				}
				// interpolate radii between sinusoids
				else
				{
					if ( sinIndex < sinusoidCount )
						sinrad_next = sin[sinIndex];
					else
						sinrad_next = sin[0];
					sinrad_prev = sin[sinIndex - 1];
					sinvert_next = vert + verticesBetweenSinusoids - (vert % verticesBetweenSinusoids);
					radius = sinrad_prev + (vert - sinvert_prev) * (sinrad_next - sinrad_prev) / (sinvert_next - sinvert_prev);
				}
				// make some noise 
				if (layer != prevInfo.index && layer != prevInfo.index + layerCount -1)
					radius += Random.Range(-1,1) * vertVariation;
				// calculate vertex position from radius
				Vector3 vertex = layerCenter + transform.forward * radius;
				
				// place handholds
				for (int i = 0; i < nodes.Length; i++ )
				{
					// skip every 3 to 8 holds
					if ( layer % Random.Range(3,8) == 0 )
					{

					}
					else if ( vert == nodes[i] )
					{
						int holdIndex = Random.Range(0, handHolds.Length);
						Quaternion holdRotation =  Quaternion.LookRotation(layerCenter - vertex, Vector3.up);
						Instantiate(handHolds[holdIndex], vertex, holdRotation);
						nodes[i] += Random.Range(-1, 1);
					}
				}
				
				// place hazards
				for ( int i = 0; i < hazardVerts.Length; i++ )
				{
					if ( layer*vertsPerLayer + vert == hazardVerts[i] )
					{
						int hazardIndex = Random.Range(0, stage.hazards.Length);
						Quaternion hazardRotation = Quaternion.LookRotation(vertex - layerCenter, Vector3.up);
						Instantiate(stage.hazards[hazardIndex], vertex * 0.9f, hazardRotation);
					}
				}

				// store vertex in verts[]
				verts[ layer*vertsPerLayer + vert] = vertex;
				uv[ layer*vertsPerLayer + vert] = new Vector2(vert, layer);
				transform.Rotate(Vector3.up, innerAngle);
			}
			layerCenter += Vector3.up * layerHeight;
		}
		layerCenter -= Vector3.up * layerHeight;
		// </vertices and uv generation>
		
		// For simplicity, fill the triangles index after creating all the vertices.
		// Each triangles[] element is an index to the vertices[] array
		// So each element in triangles[] is really indicating a point on a triangle
		// Every 3 points are the points for one triangle.
		int index = trianglesIndex;
		for ( int layer = prevInfo.index; layer < prevInfo.index + layerCount-1; layer++ )
		{
			for ( int vert = 0; vert < vertsPerLayer-1; vert++ )
			{
				// Two triangles make a square.
				// Triangle one
				triangles[index++] = (layer    *vertsPerLayer) + vert;
				triangles[index++] = (layer    *vertsPerLayer) + vert + 1;
				triangles[index++] = ((layer+1)*vertsPerLayer) + vert;
				// Triangle two
				triangles[index++] = ((layer+1)*vertsPerLayer) + vert;
				triangles[index++] = ( layer   *vertsPerLayer) + vert + 1;
				triangles[index++] = ((layer+1)*vertsPerLayer) + vert + 1;
			}
			// These two triangles join the ends of each layer
			// This is best explained by removing the code for them to see what happens...
			// (hint: look all around the mesh, easier to spot with less sides)
			triangles[index++] = ( layer   *vertsPerLayer);
			triangles[index++] = ((layer+1)*vertsPerLayer);
			triangles[index++] = ( layer   *vertsPerLayer) + vertsPerLayer-1;
			
			triangles[index++] = ((layer+1)*vertsPerLayer);
			triangles[index++] = ((layer+1)*vertsPerLayer) + vertsPerLayer-1;
			triangles[index++] = ( layer   *vertsPerLayer) + vertsPerLayer-1;
		}
		trianglesIndex = index;
		
		// Spawn a cloud layer!
		Instantiate(cloudLayer, layerCenter + Vector3.back * 10f, Quaternion.identity);
		// return last layer info for next stage
		layerInfo info = new layerInfo(prevInfo.index + layerCount, sin);
		return info;
	}
	
	void CreateMesh()
	{
		gameObject.AddComponent<MeshFilter>();
		// Assign our mesh data to the mesh
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		mesh.vertices = verts;
		mesh.uv = uv;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
		// Create a physics component for our new mesh (mesh ignores all collisions otherwise)
		gameObject.AddComponent<MeshCollider>();
		// Render dat mesh 
		gameObject.AddComponent<MeshRenderer>();
		renderer.material = stages[0].materials[0];
		renderer.castShadows = false;
		renderer.receiveShadows = false;
	}
	
}