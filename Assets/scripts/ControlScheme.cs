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
	private Hand LH;
	private Hand RH;
	private Hand LF;
	private Hand RF;
	
	void Start () 
	{
		LH = LeftHand.GetComponent<Hand>();
		RH = RightHand.GetComponent<Hand>();
	}
	

	void Update ()
	{
		if ( Input.GetButton("GripLeftHand") )
			;// LH.Grip();
		
		if ( Input.GetButton("GripRightHand") )
			;// RH.Grip();
		
		if ( Input.GetButton("GripLeftFoot") )
			;// LF.Grip();
		
		if ( Input.GetButton("GripRightFoot") )
			;// RF.Grip();
		
	}
}
