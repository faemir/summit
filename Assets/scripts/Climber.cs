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

public class Climber : MonoBehaviour 
{
	
	public string[] toolStrings = new string[] {"CAM1", "CAM2"};
	
	private GameObject[] cameras;
	private int toolNum = 0;
	private int lastToolNum;
	private Transform head;
	private Vector3 startPos;
	private float maxdist;
	// Use this for initialization
	void Start () 
	{
		startPos = transform.position;
		head = transform.FindChild("Head");
		cameras = GameObject.FindGameObjectsWithTag("MainCamera");
		foreach (GameObject cam in cameras)
		{
			cam.SetActive(false);
			if (cam.name == "ChaseCam") 
			{
				cam.SetActive(true);
			}
		}
	}
	
	// Update is called once per frame
	//tiny update
	void Update () 
	{
	
	}
	
	void OnGUI()
	{
		toolNum = GUI.Toolbar( new Rect(25,25,250,30), toolNum, toolStrings);
		
		if (toolNum != lastToolNum)
		{
			foreach (GameObject cam in cameras)
			{
				cam.SetActive(false);
			}
			cameras[toolNum].SetActive(true);
		}
		
		lastToolNum = toolNum;
		
		float distance = Vector3.Distance(startPos, head.transform.position);
		if (distance > maxdist) maxdist = distance;
		GUI.Label( new Rect(25, 60, 250, 30),"Best: " + maxdist);
	}
}
