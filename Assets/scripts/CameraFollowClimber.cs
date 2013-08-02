/* This file is part of summit
 *
 * Copyright (C) 2013 Matthew Blickem <explosivose@gmail.com>
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * You should have received a copy of the GNU General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA
 */

using UnityEngine;
using System.Collections;

public class CameraFollowClimber : MonoBehaviour 
{
	
	public Transform climber;
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 targetpos;
		targetpos.x = climber.transform.position.x * 2;
		targetpos.y = Mathf.Abs(climber.position.y - 2f);
		targetpos.z = climber.position.z * 2;
		if (targetpos.x >= 18f) targetpos.x = 18f;
		if (targetpos.x <= -18f) targetpos.x = -18f;
		if (targetpos.z >= 18f) targetpos.z = 18f;
		if (targetpos.z <= -18f) targetpos.z = -18f;
		transform.position = targetpos;
		transform.LookAt(climber.position + Vector3.up * 2f);
	}
}
