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

public class ControlScheme : MonoBehaviour 
{
	// Reference to limbs
	public Transform LeftHand;
	public Transform RightHand;
	public Transform LeftFoot;
	public Transform RightFoot;
	
	// Reference to scripts on the limbs
	private Limb LH;
	private Limb RH;
	private Limb LF;
	private Limb RF;
	
	void Start () 
	{
		Screen.lockCursor = true;
		LH = LeftHand.GetComponent<Limb>();
		RH = RightHand.GetComponent<Limb>();
		LF = LeftFoot.GetComponent<Limb>();
		RF = RightFoot.GetComponent<Limb>();
	}
	

	void Update ()
	{
		float mouseX = Input.GetAxis ("Mouse X");
		float mouseY = Input.GetAxis ("Mouse Y");
		
		/* Left Hand */
		if ( Input.GetButton("GripLeftHand") )
			
			// Player is holding grip button...
			if ( Input.GetButton("PullLeftHand") )
				// Player presses limb contract button...
				// ...whilst trying to grip the wall (successfully or not)
				// (Contract overrides Extend)
				LH.Contract();
			else if ( Input.GetButton("PushLeftHand") )
				// Player presses limb contract button...
				// ...whilst trying to grip the wall (successfully or not)
				// (Extend overrides Grip)
				LH.Extend();
			else
				// Player has not asked to Contract or Extend
				// But they do want to Grip!
				LH.Grip(mouseX, mouseY);
		else
			// Player isn't gripping, so assume they're reaching with this limb
			// Contract/Extend commands are ignored here
			LH.Reach(mouseX, mouseY);
		
		
		/* Right Hand */
		if ( Input.GetButton("GripRightHand") )

			if ( Input.GetButton("PullRightHand") )
				RH.Contract();
			else if ( Input.GetButton("PushRightHand") )
				RH.Extend();
			else
				RH.Grip(mouseX, mouseY);
		else
			RH.Reach(mouseX, mouseY);
		
		/* Left Foot */
		if ( Input.GetButton("GripLeftFoot") )
			
			if ( Input.GetButton("PullLeftFoot") )
				LF.Contract();
			else if ( Input.GetButton("PushLeftFoot") )
				LF.Extend();
			else
				LF.Grip(mouseX, mouseY);
		else
			LF.Reach(mouseX, mouseY);
		
		/* Right Foot  */
		if ( Input.GetButton("GripRightFoot") )
			if ( Input.GetButton("PullRightFoot") )
				RF.Contract();
			else if ( Input.GetButton("PushRightFoot") )
				RF.Extend();
			else
				RF.Grip(mouseX, mouseY);
		else
			RF.Reach(mouseX, mouseY);
		
	}
}
