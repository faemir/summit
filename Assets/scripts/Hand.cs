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

public class Hand : MonoBehaviour 
{
	public enum HandPos
	{
		Left,
		Right,
		None,
	}
	
	public enum GrabState
	{
		LetGo,
		Reaching,
		Grabbing,
		Grabbed,
	}
	
	public enum LimbState
	{
		Contract,
		Extend,
		Relax,
	}
	
	public HandPos thisHand = HandPos.Left;
	public Transform otherHand;
	//public Transform thisShoulder;
	public Material Green;
	public Material Orange;
	public Material Red;
	
	static float strength = 85f;
	private HandPos inputRequest;
	public GrabState grab = GrabState.Grabbing;
	private Vector3 moveForce = new Vector3(0f,0f,0f);
	private Hand otherHandScript;
	private LimbState muscle;
	
	// Use this for initialization
	void Start () 
	{
		inputRequest = HandPos.None;
		muscle = LimbState.Relax;
		renderer.material = Red;
		otherHandScript = otherHand.GetComponent<Hand>();
	}
	
	void FixedUpdate ()
	{
		Debug.DrawRay(transform.position, moveForce, Color.red);
		rigidbody.AddForce(moveForce * strength);
		Vector3 dir;
		/*switch(muscle)
		{
		case LimbState.Contract:
			if (grab == GrabState.Grabbed)
			{
				dir = transform.position - thisShoulder.transform.position;
				thisShoulder.rigidbody.AddForce(dir * strength);
			}
			else
			{
				dir = thisShoulder.transform.position - transform.position;
				rigidbody.AddForce(dir * strength);
			}
			break;
		case LimbState.Extend:
			if (grab == GrabState.Grabbed)
			{
				dir = transform.position - thisShoulder.transform.position;
				thisShoulder.rigidbody.AddForce(-dir * strength);
			}
			else
			{
				dir = thisShoulder.transform.position - transform.position;
				rigidbody.AddForce(-dir * strength);
			}
			break;
		}*/
	}
	
	// Update is called once per frame
	void Update () 
	{
		if ( Input.GetButton ("Space") ) muscle = LimbState.Contract;
		else muscle = LimbState.Relax;
		
		inputRequest = HandPos.None;
		if ( Input.GetMouseButton(0) ) inputRequest = HandPos.Left;
		else if ( Input.GetMouseButton(1) ) inputRequest = HandPos.Right;
		
		if ( inputRequest == thisHand )
		{
			grab = GrabState.Reaching;
		}
		else
		{
			if ( grab != GrabState.Grabbed )
				grab = GrabState.Grabbing;
		}
		
		if ( Input.GetMouseButton(2) )
		{
			grab = GrabState.LetGo;
		}
		
		switch (grab)
		{
		case GrabState.LetGo:
			LetGo();
			break;
		case GrabState.Reaching:
			Reach();
			break;
		case GrabState.Grabbing:
			TryGrab();
			break;
		case GrabState.Grabbed:
			break;
		}
	}
	
	void LetGo ()
	{
		//rigidbody.constraints = RigidbodyConstraints.None;
		FixedJoint[] grabs = GetComponents<FixedJoint>();
		foreach (FixedJoint g in grabs)
		{
			g.breakForce = 0f;
		}
		renderer.material = Orange;
		moveForce = Vector3.zero;
	}
	
	void Reach ()
	{
		LetGo();
		if ( otherHandScript.grab != GrabState.Grabbed ) return;
		Vector3 target;
		Vector3 mousepos = Input.mousePosition;
		Ray mouseray = Camera.main.ScreenPointToRay(mousepos);
		RaycastHit hit;
		if ( Physics.Raycast(mouseray, out hit) )
		{
			target = hit.point;
		}
		else
		{
			target = transform.position + Vector3.up;
		}
		
		Vector3 targetdir = target - transform.position;
		targetdir = (targetdir + Vector3.up * targetdir.magnitude)/2;
		Debug.DrawRay(transform.position, targetdir, Color.magenta);
		moveForce = targetdir.normalized;
	}
	
	void TryGrab ()
	{
		Vector3 walldir;
		Vector3 mousepos = Input.mousePosition;
		Ray mouseray = Camera.main.ScreenPointToRay(mousepos);
		RaycastHit hit;
		if ( Physics.Raycast(mouseray, out hit) )
		{
			walldir = hit.point - transform.position;
		}
		else
		{
			walldir = new Vector3(0f, transform.position.y + 16f, 0f) - transform.position;
		}
		moveForce = walldir.normalized;
	}
	
	void Grab()
	{
		grab = GrabState.Grabbed;
		gameObject.AddComponent<FixedJoint>();
		renderer.material = Green;
	}
	
	void Grab(Rigidbody grabme)
	{
		grab = GrabState.Grabbed;
		FixedJoint grabber = gameObject.AddComponent<FixedJoint>();
		grabber.connectedBody = grabme;
		renderer.material = Green;
	}
	
	
	void OnCollisionStay ( Collision info )
	{
		if ( grab == GrabState.Grabbing )
		{
			switch (info.collider.tag)
			{
			case "hold":
				ScoreManager.UseHold(info.transform);
				Grab();
				break;
			case "debris":
				if (info.collider.rigidbody.velocity.magnitude < 1f)
					Grab(info.collider.rigidbody);
				break;
			default:
				renderer.material = Red;
				break;
			}
		}
		else 
		{
			if ( info.collider.tag == "hold" || info.collider.tag == "debris" )
				renderer.material = Green;
		}
	}
	
}
