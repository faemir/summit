/* This file is part of summit
 *
 * Copyright (C) 2013 Daniel Cohen <analoguecolour@gmail.com>
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

public class Limb : MonoBehaviour {
	
	public enum GripState {
		LetGo,
		Gripping,
		Gripped
	}

	public enum LimbState {
		Reaching,
		Contracting,
		Extending,
		Relaxed
	}	
	public float maxReachForce = 1; // body > reachForce > gravity
	public Transform foreLimb;
	public Transform upperLimb;
	private CharacterJoint foreLimbJoint; // elbow/knee
	private CharacterJoint endLimbJoint; // hand/foot
	private CharacterJoint upperLimbJoint; // shoulder/hip
	private GripState currentGripState = GripState.LetGo;
	private LimbState currentLimbState = LimbState.Relaxed;
	//private Vector3 reachForce;
	private float limbMass;
	private Vector3 reachVelocity;
	
	// Use this for initialization
	private void Start () {
		limbMass = foreLimb.rigidbody.mass + upperLimb.rigidbody.mass + rigidbody.mass;
		foreLimbJoint = foreLimb.GetComponent<CharacterJoint>(); // elbow/knee
		endLimbJoint = transform.GetComponent<CharacterJoint>(); // hand/foot
		upperLimbJoint = upperLimb.GetComponent<CharacterJoint>(); // shoulder/hip
	}
	
	private void FixedUpdate() {
		rigidbody.AddForce(reachVelocity, ForceMode.VelocityChange);
		Debug.DrawRay(transform.position, reachVelocity);
	}
	//this is working out the movement of the limb, left/right and up/down
	private void SetReachForce(float Right, float Up) {
		
		Transform cam = Camera.main.transform;
		Vector3 reachDirection = (cam.right * Right) + (cam.up * Up);
		reachDirection = Vector3.Normalize(reachDirection); //set reachDirection magnitude to 1
		//reachForce = -Physics.gravity * limbMass;
		//reachForce += reachDirection * maxReachForce;
		reachVelocity = -rigidbody.velocity;
		reachVelocity += reachDirection * maxReachForce; //change to veloc
	}
	//wht we do when we collide with another game object
	private void OnCollisionStay(Collision info) {
		if (currentGripState == GripState.Gripping)
			switch (info.gameObject.tag) {
			case "hold":
				Grab();
				break;
			case "debris":
				if (info.collider.rigidbody.velocity.magnitude < 0.8f) // force changeme
				Grab(info.collider.rigidbody);
				break;			
		}		
	}
	//grab is called when the player tries to grab, and tries to make a fixedjoint between the hand and the wall
	private void Grab(){
		Debug.Log("Grab attempt " + gameObject.name);
		currentGripState = GripState.Gripped;
		FixedJoint activeHold = gameObject.AddComponent<FixedJoint>();
		activeHold.breakForce = 1.8f;

	}
	//this is the same as the previous, except when a parameter is passed of a game object (ie. the hold in question)
	private void Grab(Rigidbody grabme) {
	
		Debug.Log("Grab attempt " + gameObject.name);
		currentGripState = GripState.Gripped;
		FixedJoint activeHold = gameObject.AddComponent<FixedJoint>();
		activeHold.connectedBody = grabme;
		activeHold.breakForce = 1.8f;
	}
	
	
	public void Reach(float Right, float Up) {
		Relax();
		FixedJoint[] grips = GetComponents<FixedJoint>();
		foreach (FixedJoint g in grips)
		{
			g.breakForce = 0f;
		}
		currentGripState = GripState.LetGo;
		SetReachForce(Right, Up);
	}
	
	public void Grip(float Right, float Up) {
		Relax();
		if (currentGripState != GripState.Gripped) {
			currentGripState = GripState.Gripping;
			SetReachForce(Right, Up);
		}
	}
	// find the elbow, and set it's rotational limits to relax the arm
	public void Relax() {
		if (currentLimbState != LimbState.Relaxed) {
			currentLimbState = LimbState.Relaxed;
			CharacterJoint limbJoint = foreLimb.GetComponent<CharacterJoint>();
			SoftJointLimit limbHTL = limbJoint.highTwistLimit;
			SoftJointLimit limbLTL = limbJoint.lowTwistLimit;
			limbHTL.limit = 0;
			limbLTL.limit = -160;
			limbJoint.highTwistLimit = limbHTL;
			limbJoint.lowTwistLimit = limbLTL;
		}
	}
	// find the elbow, and set it's rotational limits to contract the arm
	public void Contract() {
		if (currentLimbState != LimbState.Contracting) {
			currentLimbState = LimbState.Contracting;
			CharacterJoint limbJoint = foreLimb.GetComponent<CharacterJoint>();
			SoftJointLimit limbHTL = limbJoint.highTwistLimit;
			SoftJointLimit limbLTL = limbJoint.lowTwistLimit;
			limbHTL.limit = -160;
			limbLTL.limit = -160;
			limbJoint.highTwistLimit = limbHTL;
			limbJoint.lowTwistLimit = limbLTL;
		}
	}
	// find the elbow, and set it's rotational limits to extend the arm
	public void Extend() {
		if (currentLimbState != LimbState.Extending) {
			currentLimbState = LimbState.Extending;
			CharacterJoint limbJoint = foreLimb.GetComponent<CharacterJoint>();
			SoftJointLimit limbHTL = limbJoint.highTwistLimit;
			SoftJointLimit limbLTL = limbJoint.lowTwistLimit;
			limbHTL.limit = 0;
			limbLTL.limit = 0;
			limbJoint.highTwistLimit = limbHTL;
			limbJoint.lowTwistLimit = limbLTL;
		}
	}	
}