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

/* Score Manager is a Static (not instantiable) class that keeps track of the player's score
 * The score is incremented each time the climber grabs a handhold
 * The score for grabbing a handhold depends on:
 * 	The distance from the previous handhold grabbed
 * 	The time between grabbing this hold and the previous hold
 * 	The hold is awarded zero (0) if the previous hold is higher (further from the ground)
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic; // required for C# Lists


public static class ScoreManager 
{
	public static int score = 0;
	private static List<Transform> UnusedHandholds = new List<Transform>();
	private static List<Transform> UsedHandholds = new List<Transform>();
	
	private static Vector3 lastUsePosition = Vector3.zero;
	private static float lastUseTime = 0f;
	
	public static void AddNewHold(Transform hold)
	{
		UnusedHandholds.Add(hold);
	}
	
	public static void SetHoldColour(Transform hold, Color color)
	{
		foreach ( Transform h in UnusedHandholds )
		{
			if ( h == hold )
			{
				hold.renderer.material.color = color;
			}
		}
	}
	
	public static int ScoreForHold(Transform hold)
	{
		// lastUsePosition is initialized to Vector3.zero
		if ( lastUsePosition == Vector3.zero )
			lastUsePosition = hold.position;
		
		// if the handhold has been used before then no score is awarded
		bool hasBeenUsedBefore = false;
		foreach ( Transform h in UsedHandholds )
		{
			if ( h == hold )
				hasBeenUsedBefore = true;
		}
		
		int scoreForHold = 0;
		if ( !hasBeenUsedBefore )
		{
			scoreForHold = Mathf.RoundToInt(Vector3.Distance(hold.position, lastUsePosition)*60f);
			scoreForHold = Mathf.RoundToInt(scoreForHold / (Time.time - lastUseTime));
			if ( hold.position.y < lastUsePosition.y ) scoreForHold = 0;
		}
		
		return scoreForHold;
	}
	
	public static void UseHold(Transform hold)
	{
		bool hasBeenUsedBefore = false;
		
		if ( lastUsePosition == Vector3.zero )
			lastUsePosition = hold.position;
		
		foreach ( Transform h in UsedHandholds )
		{
			if ( h == hold )
			{
				hasBeenUsedBefore = true;
			}
		}
		
		if ( !hasBeenUsedBefore )
		{
			int scoreForHold = ScoreForHold(hold);
			score += scoreForHold;
			Debug.Log ("Score = " + score + " (+" + scoreForHold + ")");
			lastUsePosition = hold.position;
			lastUseTime = Time.time;
			hold.renderer.material.color = Color.blue;
			UsedHandholds.Add(hold);
			UnusedHandholds.Remove(hold);
		}
		
	}

}
