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
	private Vector3 reachForce;
	private float limbMass;
	
	// Use this for initialization
	private void Start () {
		limbMass = foreLimb.rigidbody.mass + upperLimb.rigidbody.mass + rigidbody.mass;
		foreLimbJoint = foreLimb.GetComponent<CharacterJoint>(); // elbow/knee
		endLimbJoint = transform.GetComponent<CharacterJoint>(); // hand/foot
		upperLimbJoint = upperLimb.GetComponent<CharacterJoint>(); // shoulder/hip
	}
	
	private void FixedUpdate() {
		rigidbody.AddForce(reachForce, ForceMode.Force);
		Debug.DrawRay(transform.position, reachForce);
	}
	
	private void SetReachForce(float Right, float Up) {
		
		Transform cam = Camera.main.transform;
		Vector3 reachDirection = (cam.right * Right) + (cam.up * Up);
		reachDirection = Vector3.Normalize(reachDirection); //set reachDirection magnitude to 1
		reachForce = -Physics.gravity * limbMass;
		reachForce += reachDirection * maxReachForce;
	}
	
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
	
	private void Grab(){
		Debug.Log("Grab attempt " + gameObject.name);
		currentGripState = GripState.Gripped;
		FixedJoint activeHold = gameObject.AddComponent<FixedJoint>();
		activeHold.breakForce = 2f;

	}
	
	private void Grab(Rigidbody grabme) {
	
		Debug.Log("Grab attempt " + gameObject.name);
		currentGripState = GripState.Gripped;
		FixedJoint activeHold = gameObject.AddComponent<FixedJoint>();
		activeHold.connectedBody = grabme;
		activeHold.breakForce = 2f;
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