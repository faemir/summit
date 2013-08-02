/* This file is part of summit
 *
 * Copyright (C) 2013 Matthew Blickem <explosivose@gmail.com>
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA
 */

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
