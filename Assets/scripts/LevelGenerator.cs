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
	// Unique data for each stage
	// Serializable classes can be viewed and edited in the Unity Inspector
	[System.Serializable]
	public class StageProperties
	{
		public string name = "Default Stage";
		public Material[] materials = new Material[1];
		public Hazard[] hazards = new Hazard[1];
		public int numberOfHazards = 0;
		public int numberOfRoutes = 0;
		public float height = 10f;
		public float maxSinAmp = 2f;
		public float minSinAmp = 1f;
		public float maxSinOff = 0f;
		public float minSinOff = 0f;
		public float maxSinFrq = 0.05f;
		public float minSinFrq = 0.01f;
		public float maxSinPhs = 4f;
		public float minSinPhs = 0f;
	}
	
	// Generated mesh LevelOfDetail settings
	public enum LevelMeshLOD {High, Medium, Low};
	public LevelMeshLOD MeshDetail = LevelMeshLOD.Medium;
	
	// Tweaks for the generated mesh
	public float vertVariation = 0.125f;		// amount of noise added to each vertex
	public float minimumRadius = 2f;			// smallest permitted distance to y axis (Vector3.up)
	
	// Stuff to spawn in the level
	public bool spawnClouds = true;
	public Transform cloudLayer;				// This is the particle system prefab to Instantiate
	public Transform[] handHolds;				// A list of handhold prefabs
	
	// Data for spawning handholds for climbing routes
	public ClimbingRouteParameters[] Routes = new ClimbingRouteParameters[2];
	
	// Unique data for the mesh in each stage (see class declaration above)
	public StageProperties[] stageParemeters = new StageProperties[4];
	
	// debugMode just toggles on/off some Debug.DrawLine stuff
	public bool debugMode = false;
	
	private Transform[] stages;					// A list of the child objects containing each stage
	private float layerHeight; 					// The vertical gap between vertices in each mesh
	private int vertsPerLayer;					// The number of vertices that share the same height
	private Vector3 layerCenter = Vector3.zero; // A point in the center of the level used for placing vertices
	private int vertsBetweenSinusoids;			// The number of vertices between a 'sinusoid vertex'
	
	// Some vertices use a Sin() function to calculate their position (sinusoids).
	// The rest use a linear interpolation between two adjacent sinusoids to calculate their position.
	// If unsure, please refer to external documentation on what a sinusoid is in this context.
	private const int sinusoidCount = 8;
	
	// Each stage mesh is generated separately. To ensure these meshes line up at the seams
	// it is necessary to pass relevant information between stage generation. The relevant
	// data is passed using the following struct
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
		StartCoroutine ("Generate");
	}
	
	IEnumerator Generate()
	{
		StartCoroutine("GenerateMesh");
		yield return new WaitForSeconds(0.5f);
		SpawnHazards();
		yield return new WaitForSeconds(0.5f);
		SpawnRoutes();
	}
	
	IEnumerator GenerateMesh()
	{
		// Initialise mesh detail parameters
		switch ( MeshDetail )
		{
		case LevelMeshLOD.High:
			vertsBetweenSinusoids = 8;
			layerHeight = 0.5f;
			break;
		case LevelMeshLOD.Medium:
			vertsBetweenSinusoids = 6;
			layerHeight = 0.75f;
			break;
		case LevelMeshLOD.Low:
			vertsBetweenSinusoids = 4;
			layerHeight = 1f;
			break;
		}
		
		vertsPerLayer = vertsBetweenSinusoids * sinusoidCount;
		stages = new Transform[stageParemeters.Length];
		
		// Log mesh info
		int layerCount = 0;
		int trianglecount = 0;
		for ( int i = 0; i < stageParemeters.Length; i++ )
		{
			layerCount += Mathf.CeilToInt((stageParemeters[i].height + layerHeight) / layerHeight);
			trianglecount += (layerCount * vertsPerLayer * 6);
		}
		Debug.Log ("[MeshGen] Layer count: " + layerCount);
		Debug.Log ("[MeshGen] Triangle count: " + trianglecount);
		
		// Initialise layerInfo struct
		layerInfo info;
		info.index = 0;
		info.sinValues = new float[sinusoidCount];
		for ( int i = 0; i < sinusoidCount; i++)
		{
			info.sinValues[i] = Random.Range(stageParemeters[0].minSinOff, stageParemeters[0].maxSinOff);
		}
		
		// Generate meshes for each stage
		Mesh mesh;
		for ( int i = 0; i < stageParemeters.Length; i++)
		{
			mesh = GenerateMeshData(stageParemeters[i], ref info);
			stages[i] = CreateMesh(stageParemeters[i], mesh);
			yield return new WaitForFixedUpdate();
		}
		if (debugMode)
			Debug.DrawLine(Vector3.zero, layerCenter, Color.white, Mathf.Infinity);
		
	}
	
	Mesh GenerateMeshData(StageProperties stage, ref layerInfo prevInfo)
	{
		int layerCount = Mathf.CeilToInt((stage.height + layerHeight) / layerHeight);
		int vertCount = layerCount * vertsPerLayer;
		float innerAngle = 360f / (float)vertsPerLayer;
		
		Vector3[] verts = new Vector3[vertCount];
		Vector2[] uv = new Vector2[vertCount];
		int[] triangles = new int[vertCount * 6];
		
		// <sinusoid parameters>
		float[] freq = new float[sinusoidCount];
		for ( int i = 0; i < sinusoidCount; i++)
			freq[i] = Random.Range(stage.minSinFrq, stage.maxSinFrq);
		
		float[] amp = new float[sinusoidCount];
		for ( int i = 0; i < sinusoidCount; i++)
			amp[i] = Random.Range(stage.minSinAmp, stage.maxSinAmp);
		
		float[] off = new float[sinusoidCount];
		for ( int i = 0; i < sinusoidCount; i++)
			off[i] = Random.Range(stage.minSinOff, stage.maxSinOff);
		
		float[] phas = new float[sinusoidCount];
		for ( int i = 0; i < sinusoidCount; i++)
			phas[i] = Random.Range(stage.minSinPhs, stage.maxSinPhs);
		// </sinusoid parameters>
		
		// <offset corrections>
		float[] sin = new float[sinusoidCount];
		for ( int i = 0; i < sinusoidCount; i++ )
		{
			float height = (float)(prevInfo.index) * layerHeight;
			sin[i] = amp[i] * Mathf.Sin(2f * Mathf.PI  * freq[i] * height + phas[i]) + off[i];
			off[i] += prevInfo.sinValues[i] - sin[i];
		}
		// </offset corrections>
		
		// <vertices and uv generation>
		float radius = 0f;
		float sinrad_next; float sinrad_prev;
		float sinvert_next; float sinvert_prev = 0f;
		for ( int layer = 0; layer < layerCount; layer++ )
		{
			// calculate next sinusoid value 
			float height = (float)(layer + prevInfo.index) * layerHeight;
			for ( int i = 0; i < sinusoidCount; i++ )
			{
				sin[i] = Mathf.Abs(amp[i] * Mathf.Sin(2f * Mathf.PI  * freq[i] * height + phas[i]) + off[i] - minimumRadius) + minimumRadius;
			}
			
			int sinIndex = 0;
			for ( int vert = 0; vert < vertsPerLayer; vert++ )
			{
				// insert sinusoid values as radii
				if ( vert % vertsBetweenSinusoids == 0 ) 
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
					sinvert_next = vert + vertsBetweenSinusoids - (vert % vertsBetweenSinusoids);
					radius = sinrad_prev + (vert - sinvert_prev) * (sinrad_next - sinrad_prev) / (sinvert_next - sinvert_prev);
				}
				// make some noise 
				if (layer != 0 && layer != layerCount-1)
					radius += Random.Range(-1,1) * vertVariation;
				
				// calculate vertex position from radius
				Vector3 vertex = layerCenter + transform.forward * radius;
				
				// store vertex in verts[]
				verts[ layer*vertsPerLayer + vert] = vertex;
				uv[ layer*vertsPerLayer + vert] = new Vector2(vert, layer);
				transform.Rotate(Vector3.up, innerAngle);
			}
			uv[ layer*vertsPerLayer + vertsPerLayer-1] = new Vector2(vertsPerLayer, layer);
			layerCenter += Vector3.up * layerHeight;
		}
		layerCenter -= Vector3.up * layerHeight;
		// </vertices and uv generation>
		
		// Fill the triangles index after creating all the vertices.
		// Each triangles[] element is an index to the vertices[] array
		// So each element in triangles[] is really indicating a point on a triangle
		// Every 3 points are the points for one triangle.
		int index = 0;
		for ( int layer = 0; layer < layerCount-1; layer++ )
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
			// (hint: look all around the mesh, easier to spot with less verts)
			triangles[index++] = ( layer   *vertsPerLayer);
			triangles[index++] = ((layer+1)*vertsPerLayer);
			triangles[index++] = ((layer+1)*vertsPerLayer) + vertsPerLayer-1;
			
			triangles[index++] = ( layer   *vertsPerLayer);
			triangles[index++] = ((layer+1)*vertsPerLayer) + vertsPerLayer-1;
			triangles[index++] = ( layer   *vertsPerLayer) + vertsPerLayer-1;
		}
		
		// Spawn a cloud layer!
		if (spawnClouds)
			Instantiate(cloudLayer, layerCenter + Vector3.back * 10f, Quaternion.identity);
		
		// return 
		prevInfo = new layerInfo(prevInfo.index + layerCount, sin);
		Mesh m = new Mesh();
		m.vertices = verts;
		m.uv = uv;
		m.triangles = triangles;
		return m;
	}
	
	Transform CreateMesh(StageProperties stage, Mesh mesh)
	{
		GameObject child = new GameObject(stage.name);
		child.transform.parent = transform;
		child.tag = "wall";
		child.AddComponent<MeshFilter>();
		Mesh childmesh = child.GetComponent<MeshFilter>().mesh;
		// Assign our mesh data to the mesh
		childmesh.Clear();
		childmesh.vertices = mesh.vertices;
		childmesh.uv = mesh.uv;
		childmesh.triangles = mesh.triangles;
		childmesh.RecalculateNormals();
		// Create a physics component for our new mesh (mesh ignores all collisions otherwise)
		child.AddComponent<MeshCollider>();
		// Render dat mesh 
		child.AddComponent<MeshRenderer>();
		child.renderer.material = stage.materials[0];
		child.renderer.castShadows = true;
		child.renderer.receiveShadows = true;
		return child.transform;
	}
	
	// Data class for defining different types of hand hold routes
	public enum RouteStyle {LockedToStartAngle, FreeMoving};
	[System.Serializable]
	public class ClimbingRouteParameters
	{
		public string name = "Default Route";
		public int instancesOfThisRoute = 4;
		public float minAngleVariance = -10f;
		public float maxAngleVariance = 10f;
		public RouteStyle style = RouteStyle.LockedToStartAngle;
	}
	
	void SpawnRoutes()
	{
		int numberOfRoutes = 0;
		int handholdCount = 0;
		for (int i = 0; i < Routes.Length; i++) numberOfRoutes += Routes[i].instancesOfThisRoute;
		
		int[] shuffledRoutes = new int[numberOfRoutes];
		
		for (int i = 0; i < numberOfRoutes; i++)
		{
			shuffledRoutes[i] = i+1;
		}
		
		for (int i = 0; i < numberOfRoutes; i++)
		{
			int roll = shuffledRoutes[Random.Range(1, numberOfRoutes - i)];
			for (int u = 0; u < numberOfRoutes-i-1; u++)
			{
				if (roll == shuffledRoutes[u])
				{
					shuffledRoutes[u] = shuffledRoutes[shuffledRoutes.Length-i-1];
					shuffledRoutes[shuffledRoutes.Length-i-1] = roll;
					break;
				}
			}
		}
		
		float[] startAngles = new float[numberOfRoutes];
		
		for (int i = 0; i < numberOfRoutes; i++)
		{
			startAngles[i] = shuffledRoutes[i]*360f / numberOfRoutes;
		}
		
		int routeIndex = 0;
		for (int i = 0; i < Routes.Length; i++)
		{
			for (int u = 0; u  < Routes[i].instancesOfThisRoute; u++)
			{
				handholdCount += SpawnSingleRoute(Routes[i].style, startAngles[routeIndex++], Routes[i].minAngleVariance, Routes[i].maxAngleVariance);
			}
		}
		Debug.Log ("Number of HandHolds: " + handholdCount);
	}
	
	int SpawnSingleRoute(RouteStyle style, float startAngle, float minDeltaAngle, float maxDeltaAngle)
	{
		int handholdCount = 0;
		Transform hold;
		
		float maxHeight = 0f;
		for (int i = 0; i < stageParemeters.Length; i++ )
		{
			maxHeight += stageParemeters[i].height;
		}
		
		float height = 0f;
		float nextStageHeight = stageParemeters[0].height;
		
		int stageIndex = 0;
		int holdIndex = 0;
		float noiseyAngle = startAngle;
		while (height < maxHeight) 
		{
			if (style == RouteStyle.LockedToStartAngle)
				noiseyAngle = startAngle + Random.Range(minDeltaAngle, maxDeltaAngle);
			if (style == RouteStyle.FreeMoving)
				noiseyAngle += Random.Range(minDeltaAngle, maxDeltaAngle);
			
			float minDeltaHeight = Mathf.Lerp(0.1f, 0.5f, height/maxHeight);
			float maxDeltaHeight = Mathf.Lerp(0.8f, 2f, height/maxHeight);
			height += Random.Range(minDeltaHeight, maxDeltaHeight);
			if ( height > nextStageHeight ) 
			{
				stageIndex++;
				if ( stageIndex >= stageParemeters.Length ) stageIndex = stageParemeters.Length - 1;
				nextStageHeight+= stageParemeters[stageIndex].height;
			}
			
			Vector3 castDestination = Vector3.up * height;
			Vector3 castOrigin = new Vector3();
			castOrigin.x = Mathf.Sin(Mathf.Deg2Rad * noiseyAngle) * 50f;
			castOrigin.y = height;
			castOrigin.z = Mathf.Cos(Mathf.Deg2Rad * noiseyAngle) * 50f;
			
			
			Ray cast = new Ray(castOrigin, castDestination - castOrigin);
			RaycastHit hit;
			
			Color debugColor = Color.red;
			
			if (Physics.Raycast(cast, out hit))
			{
				if (hit.transform.tag == "wall")
				{
					debugColor = Color.green;
					holdIndex = Random.Range(0, handHolds.Length);
					Quaternion spawnRotation = Quaternion.LookRotation(hit.normal);
					hold = Instantiate(handHolds[holdIndex], hit.point, spawnRotation) as Transform;
					hold.parent = stages[stageIndex];
					hold.renderer.material = stageParemeters[stageIndex].materials[0];
					ScoreManager.AddNewHold(hold);
					handholdCount++;
				}
				if (debugMode)
					Debug.DrawRay(hit.point, hit.normal, debugColor, Mathf.Infinity);
			}
		}
		return handholdCount;
	}
	
	// Data class for defining hazards to spawn
	[System.Serializable]
	public class Hazard
	{
		public string name = "Default Hazard";
		public Transform hazardPrefab;
		public int numberOfInstances = 1;
	}
	
	void SpawnHazards()
	{
		int hazardCount = 0;
		float angleOffset = Random.Range(0f,360f);
		float minHeight = 0f;
		float maxHeight = 0f;
		for ( int i = 0; i < stageParemeters.Length; i++)
		{
			minHeight = maxHeight;
			maxHeight += stageParemeters[i].height;
			for ( int h = 0; h < stageParemeters[i].hazards.Length; h++)
			{
				int numberOfInstances = stageParemeters[i].hazards[h].numberOfInstances;
				for (int instance = 0; instance < numberOfInstances; instance++)
				{
					float angle = instance * 360f / numberOfInstances;
					angle += angleOffset;
					angle += Random.Range(-30f, 30f);
					float height = Random.Range(minHeight, maxHeight);
					if ( SpawnSingleHazard(stageParemeters[i].hazards[h].hazardPrefab, angle, height, i) )
						hazardCount++;
				}
			}
		}
		Debug.Log ("Hazards Spawned: " + hazardCount);
	}
	
	bool SpawnSingleHazard(Transform hazardPrefab, float angle, float height, int stage)
	{
		bool spawned = false;
		Vector3 castDestination = Vector3.up * height;
		Vector3 castOrigin = new Vector3();
		castOrigin.x = Mathf.Sin(Mathf.Deg2Rad * angle) * 50f;
		castOrigin.y = height;
		castOrigin.z = Mathf.Cos(Mathf.Deg2Rad * angle) * 50f;
		Ray cast = new Ray(castOrigin, castDestination - castOrigin);
		RaycastHit hit;
		Color debugColor = Color.red;
		if (Physics.Raycast(cast, out hit))
		{
			if (hit.transform.tag == "wall")
			{
				debugColor = Color.magenta;
				Quaternion spawnRotation = Quaternion.LookRotation(hit.normal);
				hazardPrefab = Instantiate(hazardPrefab, hit.point, spawnRotation) as Transform;
				hazardPrefab.parent = stages[stage];
				spawned = true;
			}
			if (debugMode)
				Debug.DrawRay(hit.point, hit.normal, debugColor, Mathf.Infinity);
		}
		return spawned;
	}
	
	// Extract normals array from all generated meshes.
	Vector3[] GetGeneratedNormals()
	{
		int vertexCount = 0;
		Transform[] children = new Transform[stageParemeters.Length];
		Vector3[][] stagenormals = new Vector3[stageParemeters.Length][];
		Mesh mesh;
		
		for ( int i = 0; i < stageParemeters.Length; i++)
		{
			children[i] = transform.FindChild(stageParemeters[i].name);
			if ( children[i] != null )
			{
				mesh = children[i].GetComponent<MeshFilter>().mesh;
				stagenormals[i] = mesh.normals;
				vertexCount += mesh.normals.Length;
			}
		}
		
		Vector3[] normals = new Vector3[vertexCount];
		vertexCount = 0;
		for (int i = 0; i < stageParemeters.Length; i++)
		{
			stagenormals[i].CopyTo(normals, vertexCount);
			vertexCount += stagenormals[i].Length;
		}
		return normals;
	}
	
	// Extract vertices array from all generated meshes.
	Vector3[] GetGeneratedVertices()
	{
		int vertexCount = 0;
		Transform[] children = new Transform[stageParemeters.Length];
		Vector3[][] stagevertices = new Vector3[stageParemeters.Length][];
		Mesh mesh;
		for ( int i = 0; i < stageParemeters.Length; i++)
		{
			children[i] = transform.FindChild(stageParemeters[i].name);
			if ( children[i] != null )
			{
				mesh = children[i].GetComponent<MeshFilter>().mesh;
				stagevertices[i] = mesh.vertices;
				vertexCount += mesh.vertices.Length;
			}
		}
		
		Vector3[] verts = new Vector3[vertexCount];
		vertexCount = 0;
		for ( int i = 0; i < stageParemeters.Length; i++)
		{
			stagevertices[i].CopyTo(verts, vertexCount);
			vertexCount += stagevertices[i].Length;
		}
		return verts;
	}
}


