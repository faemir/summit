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
