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

using UnityEngine;
using System.Collections;

public class CameraFollowClimber : MonoBehaviour 
{
	
	public Transform climber;
	public float followDistance = 1.5f;
	
	void Start ()
	{
		if ( !climber )
		{
			Transform climberParent = GameObject.FindGameObjectWithTag("climber").transform;
			climber = climberParent.FindChild("Head");
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 lookDirection = (Vector3.up * climber.position.y) - climber.position;
		Vector3 position = new Vector3(climber.position.x * followDistance, climber.position.y, climber.position.z * followDistance); 
		transform.position = position;
		transform.rotation = Quaternion.LookRotation(lookDirection);
	}
}
