using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour 
{
	public Transform sidePrefab;
	public Material material;
	public float startRadius = 10f;
	public int startSides = 16;
	public int startLayers = 32;
	public float maxLean = 1f;
	public float maxPinch = 1f;
	
	private float radius;
	private int sides;
	private int layers;
	private float layerHeight;
	private float innerAngle = 0f;
	private Vector3 columnHead = Vector3.zero;
	private Vector3 columnHead2 = Vector3.up;
	private float sideLength;
	
	void Start () 
	{
		// Initial values
		radius = startRadius;
		sides = startSides;
		layers = startLayers;
		innerAngle = 360f / (float)sides;
		sideLength = 2f * radius * Mathf.Tan(Mathf.Deg2Rad * 180f / (float) sides);
		Vector3 prefabScale = sidePrefab.localScale;
		prefabScale.x = sideLength;
		sidePrefab.localScale = prefabScale;
		layerHeight = prefabScale.y;
		// Start generation
		StartCoroutine("Generate");
	}
	
	Vector3[] vertices;
	Vector2[] uv;
	Vector4[] tangents;
	int[] triangles;
	
	IEnumerator Generate()
	{
		int side = 0;
		int layer = 0;
		Vector3 sideCenter;
		Vector3 layerClockHand;
		Vector3 nextColumnHead;
		Vector3 prefabScale;
		
		gameObject.AddComponent<MeshFilter>();
		gameObject.AddComponent<MeshRenderer>();
		renderer.material = material;
		
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		vertices = new Vector3[layers * sides];
		uv = new Vector2[layers * sides];
		tangents = new Vector4[layers * sides];
		triangles = new int[layers * sides * 6];
		int index = 0;
		for ( layer = 0; layer < layers; layer++ )
		{
			for ( side = 0; side < sides; side++ )
			{
				Vector3 vertex = columnHead + (transform.forward * radius);
				vertices[ layer*sides + side ] = vertex;
				uv[layer*sides + side] = new Vector2(side, layer);
				Vector3 r = new Vector3(Random.Range(-0.01f,0.01f),0f,Random.Range(-0.01f,0.01f));
				transform.Rotate(Vector3.up, innerAngle);
			}
			nextColumnHead = new Vector3(Random.Range(-maxLean,maxLean), layerHeight, Random.Range(-maxLean, maxLean));
			columnHead += nextColumnHead;
			radius += Random.Range(-maxPinch, maxPinch);
		}
		
		for ( layer =0; layer < layers-1; layer++ )
		{
			for ( side = 0; side < sides-1; side++ )
			{
				triangles[index++] = (layer    *sides) + side;
				triangles[index++] = (layer    *sides) + side + 1;
				triangles[index++] = ((layer+1)*sides) + side;
				
				triangles[index++] = ((layer+1)*sides) + side;
				triangles[index++] = ( layer   *sides) + side + 1;
				triangles[index++] = ((layer+1)*sides) + side + 1;
			}
			
			triangles[index++] = ( layer   *sides);
			triangles[index++] = ((layer+1)*sides);
			triangles[index++] = ( layer   *sides) + sides-1;
			
			triangles[index++] = ((layer+1)*sides);
			triangles[index++] = ((layer+1)*sides) + sides-1;
			triangles[index++] = ( layer   *sides) + sides-1;
		}
		
		
		
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;
		
		mesh.RecalculateNormals();
		
		/*
		for ( layer = 0; layer<layers; layer++ )
		{
			for ( side=0; side<sides; side++ )
			{
				layerClockHand = transform.forward * radius;
				sideCenter = columnHead + layerClockHand;
				Instantiate(sidePrefab, sideCenter, Quaternion.LookRotation(layerClockHand));
				
				transform.Rotate(Vector3.up, innerAngle);
			}
			nextColumnHead = new Vector3(Random.Range(-maxLean,maxLean), layerHeight, Random.Range(-maxLean, maxLean));
			columnHead += nextColumnHead;
			radius += Random.Range(-maxPinch, maxPinch);
			sideLength = 2f * radius * Mathf.Tan(Mathf.Deg2Rad * 180f / (float) sides);
			prefabScale = sidePrefab.localScale;
			prefabScale.x = sideLength;
			sidePrefab.localScale = prefabScale;
			yield return new WaitForFixedUpdate();
		}
		*/
		
		yield return new WaitForSeconds(0.1f);
	}
}