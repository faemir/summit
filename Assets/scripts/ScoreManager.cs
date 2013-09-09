using UnityEngine;
using System.Collections;
using System.Collections.Generic;


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
	
	public static void UseHold(Transform hold)
	{
		bool hasBeenUsedBefore = false;
		UnusedHandholds.Remove(hold);
		
		if ( lastUsePosition == Vector3.zero )
		{
			lastUsePosition = hold.position;
		}
		
		foreach ( Transform h in UsedHandholds )
		{
			if ( h == hold )
			{
				hasBeenUsedBefore = true;
			}
		}
		
		if ( !hasBeenUsedBefore )
		{
			UsedHandholds.Add (hold);
			
			int scoreForHold = Mathf.RoundToInt(Vector3.Distance(hold.position, lastUsePosition) * 60f);
			scoreForHold = Mathf.RoundToInt(scoreForHold / (Time.time - lastUseTime));
			if ( hold.position.y < lastUsePosition.y ) scoreForHold = 0;
			score += scoreForHold;
			Debug.Log ("Score = " + score + " (+" + scoreForHold + ")");
			lastUsePosition = hold.position;
			lastUseTime = Time.time;
			hold.renderer.material.color = Color.blue;
		}
	}

}
