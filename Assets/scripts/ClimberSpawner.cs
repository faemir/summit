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

/* ClimberSpawner allows the player to choose their climber and starting position
 * This is a component on the main camera
 * 
 */

using UnityEngine;
using System.Collections;

public class ClimberSpawner : MonoBehaviour {
	
	public Transform Level;
	public Transform[] climbers = new Transform[1];
	public Transform[] climberGhosts = new Transform[1];
	
	private LevelGenerator levelGen;
	private Vector3[] bottomVerts;
	private Quaternion lastRotation;
	private Quaternion nextRotation;
	private Vector3 lastPosition;
	private Vector3 nextPosition;
	private float lastMoveTime = 0f;
	private float cameraMoveSpeed = 6f;
	private int climberIndex = 0;
	private int vertIndex = 0;
	
	private Transform activeGhost;
	private Quaternion lastGhostRotation;
	private Quaternion nextGhostRotation;
	private Vector3 lastGhostPosition;
	private Vector3 nextGhostPosition;
	private float lastChangeTime;
	
	// Use this for initialization
	void Start () 
	{
		if (climbers.Length != climberGhosts.Length) 
			Debug.LogError("Spawn Climber: There must be a ghost for each climber!");
		
		// use the bottom vertices of the generated level to decide where to move the camera
		levelGen = Level.GetComponent<LevelGenerator>();
		bottomVerts = levelGen.GetBottomVertices();
		cameraMoveSpeed = bottomVerts.Length / 8f;
		
		activeGhost = Instantiate(climberGhosts[climberIndex], transform.position, transform.rotation) as Transform;
		UpdateBaseVert();
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		// To change climber character the player uses the mouse scrollwheel
		float changeCharacter = Input.GetAxisRaw("Mouse ScrollWheel");
		
		// Only change character every x seconds
		if ( Time.time - lastChangeTime > 1f )
		{
			// scrollwheel up
			if ( changeCharacter > 0f )
			{
				climberIndex++;
				if (climberIndex > climbers.Length-1)
					climberIndex = 0;
			}
			
			// scrollwheel down
			if ( changeCharacter < 0f )
			{
				climberIndex--;
				if (climberIndex < 0)
					climberIndex = climbers.Length-1;
			}
			
			// if scroll wheel has changed position then switch climber
			if ( changeCharacter != 0f )
				SwitchClimber();
		}
		
		// to change start position the player uses the horizontal axis (i.e. left/right)
		float moveAxis = Input.GetAxisRaw("Horizontal");
		
		// this statement prevents changes in vertIndex whilst the camera is still moving
		if ( Time.time - lastMoveTime > 1f/cameraMoveSpeed )
		{
			// move left
			if ( moveAxis < 0f )
			{
				vertIndex++;
				if (vertIndex > bottomVerts.Length-1)
					vertIndex = 0;
			}
			
			// move right
			if ( moveAxis > 0f )
			{
				vertIndex--;
				if (vertIndex < 0)
					vertIndex = bottomVerts.Length-1;
			}
			
			// if player requests movement, update the target positions and rotations
			if ( moveAxis != 0f )
				UpdateBaseVert();
		}
		
		// lerp (move smoothly) between last position to the next position
		transform.position = Vector3.Lerp(lastPosition, nextPosition, (Time.time - lastMoveTime)*cameraMoveSpeed);
		transform.rotation = Quaternion.Lerp(lastRotation, nextRotation, (Time.time - lastMoveTime)*cameraMoveSpeed);
		Debug.DrawLine(transform.position, nextPosition);
		
		activeGhost.position = Vector3.Lerp(lastGhostPosition, nextGhostPosition, (Time.time - lastMoveTime)*cameraMoveSpeed);
		activeGhost.rotation = Quaternion.Lerp(lastGhostRotation, nextGhostRotation, (Time.time - lastMoveTime)*cameraMoveSpeed);
		Debug.DrawLine(activeGhost.position, nextGhostPosition);
		
		// if the player hits the spacebar, the game starts!
		if ( Input.GetKey (KeyCode.Space) )
			SpawnClimber();
	}
	
	// Updates the target positions and rotations of the camera and 'ghost' climber 
	// positions and rotations are based on bottomverts[vertindex] position 
	void UpdateBaseVert()
	{
		// camera position and rotation
		lastPosition = transform.position;
		lastRotation = transform.rotation;
		nextPosition = (bottomVerts[vertIndex] * 2f) + Vector3.up;
		nextRotation = Quaternion.LookRotation(bottomVerts[vertIndex] + Vector3.up - nextPosition);
		lastMoveTime = Time.time;
		
		// climber ghost position and rotation
		lastGhostPosition = activeGhost.position;
		lastGhostRotation = activeGhost.rotation;
		nextGhostPosition = (bottomVerts[vertIndex] * 1.1f);
		nextGhostRotation = Quaternion.LookRotation(bottomVerts[vertIndex] - nextGhostPosition);
	}
	
	// destroy the active ghost and spawn the next one using climberIndex
	void SwitchClimber()
	{
		Debug.Log ("Switching climber to " + climbers[climberIndex].name + " (" + climberIndex + ")");
		Vector3 spawnPos = activeGhost.position;
		Quaternion spawnRot = activeGhost.rotation;
		Destroy(activeGhost.gameObject);
		activeGhost = Instantiate(climberGhosts[climberIndex], spawnPos, spawnRot) as Transform;
		lastChangeTime = Time.time;
	}
	
	// Initiates the CameraFollowClimber component, and disables this ClimberSpawner component
	void SpawnClimber()
	{
		CameraFollowClimber camScript = GetComponent<CameraFollowClimber>();
		camScript.Climber = Instantiate(climbers[climberIndex], activeGhost.position, activeGhost.rotation) as Transform;
		Destroy(activeGhost.gameObject);
		camScript.enabled = true;
		this.enabled = false;
	}
}
