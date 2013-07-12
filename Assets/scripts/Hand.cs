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
	
	public GrabState MyProperty { get; set; }
	
	public HandPos thisHand = HandPos.Left;
	public Transform otherHand;
	public Material Green;
	public Material Orange;
	public Material Red;
	
	static float strength = 80f;
	private HandPos inputRequest;
	public GrabState grab = GrabState.Grabbing;
	private Vector3 moveForce = Vector3.zero;
	private Hand otherHandScript;
	
	// Use this for initialization
	void Start () 
	{
		inputRequest = HandPos.None;
		renderer.material = Red;
		otherHandScript = otherHand.GetComponent<Hand>();
	}
	
	void FixedUpdate ()
	{
		Debug.DrawRay(transform.position, moveForce, Color.red);
		rigidbody.AddForce(moveForce * strength);
	}
	
	// Update is called once per frame
	void Update () 
	{	
		inputRequest = HandPos.None;
		if ( Input.GetKey(KeyCode.Q) ) inputRequest = HandPos.Left;
		else if ( Input.GetKey(KeyCode.E) ) inputRequest = HandPos.Right;
		
		if ( inputRequest == thisHand )
		{
			grab = GrabState.Reaching;
		}
		else
		{
			if ( grab != GrabState.Grabbed )
				grab = GrabState.Grabbing;
		}
		
		if ( Input.GetKey(KeyCode.W) )
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
			case "wall":
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
			if ( info.collider.tag == "wall" || info.collider.tag == "debris" )
				renderer.material = Green;
		}
	}
	
}
