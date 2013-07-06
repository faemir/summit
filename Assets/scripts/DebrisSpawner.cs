using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebrisSpawner : MonoBehaviour 
{
	public Transform[] debris;

	// Use this for initialization
	void Start () 
	{
		StartCoroutine("SpawnDebris");
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	IEnumerator SpawnDebris()
	{
		while (true)
		{
			int index = Random.Range(0, debris.Length);
			Transform newDebris = (Transform)Instantiate(debris[index],transform.position + Random.onUnitSphere * 2, Random.rotation);
			newDebris.name = "Debris";
			Vector3 newScale = new Vector3(Random.Range(0.5f, 4f), Random.Range(0.5f, 4f), Random.Range(0.5f, 4f));
			newDebris.transform.localScale = newScale;
			newDebris.rigidbody.mass = Random.Range (20,40);
			yield return new WaitForSeconds(1f);
		}
	}
}
