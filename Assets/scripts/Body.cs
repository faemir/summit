using UnityEngine;
using System.Collections;

public class Body : MonoBehaviour {
	
	public Transform RightHand;
	public Transform LeftHand;	
	public Transform RightFoot;
	public Transform LeftFoot;
	public Transform RightLeg;
	public Transform LeftLeg;

	private Hand RightHandScript;
	private Hand LeftHandScript;
	private Foot RightFootScript;
	private Foot LeftFootScript;
	
	private float strength = 80f;
	static float bodyStrengthWeak = 50f;
	static float bodyStrengthStrong = 120f;
	static float legStrength = 20f;
	private Vector3 moveForce = Vector3.zero;
	private Vector3 oldMousePos = Vector3.zero;
	
	private bool ControlBody = false;
	
	public GrabState MyProperty { get; set; }
	
	
	// Use this for initialization
	void Start () {
		RightHandScript = RightHand.GetComponent<Hand>();
		LeftHandScript = LeftHand.GetComponent<Hand>();
		
		RightFootScript = RightFoot.GetComponent<Foot>();
		LeftFootScript = LeftFoot.GetComponent<Foot>();
	}
	
	void FixedUpdate ()
	{
		Debug.DrawRay(transform.position, moveForce, Color.red);
		rigidbody.AddForce(moveForce * strength);
	}
	
	// Update is called once per frame
	void Update () {
	
		if( Input.GetKey(KeyCode.Space)) ControlBody = true;
		else{
			ControlBody = false;
			oldMousePos = new Vector3(0f,0f,0f);
		}
		
		if ( RightHandScript.grab == GrabState.Grabbed &&
			LeftHandScript.grab == GrabState.Grabbed &&
			RightFootScript.grab == GrabState.Grabbed &&
			LeftFootScript.grab == GrabState.Grabbed )
		{
			HoldBody();
		}
		else
		{
			MoveBody();
		}
	}
	
	void HoldBody(){
		if ( ControlBody == true ){
						
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
			strength = bodyStrengthStrong;
		}
		else{
			Vector3 targetdir = Camera.main.transform.position - transform.position;
			targetdir = (targetdir + Vector3.up * targetdir.magnitude)/2;
			moveForce = targetdir.normalized;
			strength = bodyStrengthStrong;
			LeftLeg.rigidbody.AddForce(moveForce*legStrength);
			RightLeg.rigidbody.AddForce(moveForce*legStrength);
		}
	}
	
	void MoveBody(){
		moveForce = Vector3.up;
		strength = bodyStrengthWeak;
	}
}
