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

public class Fred : MonoBehaviour 
{
	
	public enum Kinematic
	{
		None,
		Head,
		UpperTorso,
		BothHands,
		EntireBody,
	}
	
	public bool useGravity = true;
	public Kinematic MakeKinematic = Kinematic.None;
	public float Strength = 5f;
	
	private bool useGravityOld;
	private Kinematic MakeKinematicOld;
	private float OldStrength;
	// Use this for initialization
	void Start () 
	{
		SetGravity();
		SetKinematic();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (useGravity != useGravityOld)
			SetGravity();
		useGravityOld = useGravity;
		if (MakeKinematic != MakeKinematicOld)
			SetKinematic();
		MakeKinematicOld = MakeKinematic;
		if (Strength != OldStrength)
			SetStrength();
		OldStrength = Strength;
	}
	
	void SetGravity()
	{
		Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
		foreach (Rigidbody r in rigidbodies) 
		{
			r.useGravity = useGravity;
		}
	}
	
	void SetKinematic()
	{
		Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
		foreach (Rigidbody r in rigidbodies) 
		{
			r.isKinematic = false;
		}
		
		switch (MakeKinematic)
		{
		case Kinematic.None:
			break;
		case Kinematic.Head:
			foreach (Rigidbody r in rigidbodies)
			{
				if (r.transform.name == "Head")
					r.isKinematic = true;
			}
			break;
		case Kinematic.UpperTorso:
			foreach (Rigidbody r in rigidbodies)
			{
				if (r.transform.name == "UpperTorso")
					r.isKinematic = true;
			}
			break;
		case Kinematic.BothHands:
			foreach (Rigidbody r in rigidbodies)
			{
				if (r.transform.name == "RightHand" || r.transform.name == "LeftHand")
					r.isKinematic = true;
			}
			break;
		case Kinematic.EntireBody:
			foreach (Rigidbody r in rigidbodies)
			{
				r.isKinematic = true;
			}
			break;
		}

	}
	
	void SetStrength()
	{
		Limb[] limbs = GetComponentsInChildren<Limb>();
		foreach (Limb l in limbs)
		{
			l.maxReachForce = Strength;
		}
	}
}
