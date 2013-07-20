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
	private Vector3 reachDirection;
	private float limbMass;
	
	// Use this for initialization
	private void Start () {
		limbMass = foreLimb.rigidbody.mass + upperLimb.rigidbody.mass + rigidbody.mass;
		foreLimbJoint = foreLimb.GetComponent<CharacterJoint>(); // elbow/knee
		endLimbJoint = endLimb.GetComponent<CharacterJoint>(); // hand/foot
		upperLimbJoint = upperLimb.GetComponent<CharacterJoint>(); // shoulder/hip
	}
	
	private void fixedUpdate() {
		Vector3 reachForce = Vector3.up * (limbMass * 9.81);
		reachForce += reachDirection * maxReachForce;
		rigidbody.AddForce(reachForce);
	}
	private void SetReachDirection(float Right, float Up) {
		Transform cam = Camera.main.transform;
		reachDirection = (cam.right * Right) + (Vector3.up * Up);
		reachDirection = Vector3.Normalize(reachDirection); //set reachDirection magnitude to 1
	}
	
	private void OnCollisionStay(Collision info) {
		if (currentGripState = GripState.Gripping)
			switch (info.gameObject.tag) {
			case "hold":
				grab();
				break;
			case "debris":
				if (info.collider.rigidbody.velocity.magnitude < 0.8f) // force changeme
				Grab(info.collider.rigidbody);
				break;			
		}		
	}
	
	private void grab(){
		if (info.gameObject.tag == "hold") {
			grab = GrabState.Grabbed;
			gameObject.AddComponent<FixedJoint>();
		}
	}
	
	private void grab(Rigidbody grabme) {
		if (info.gameObject.tag == "debrishold") {
			currentGripState = GripState.Gripped;
			FixedJoint activeHold = gameObject.AddComponent<FixedJoint>();
			activeHold.connectedBody = grabme;
		}
	}
	
	
	public void Reach(float Right, float Up) {
		if (currentGripState != GripState.LetGo) {
			currentGripState = GripState.LetGo;
			reachDirection = SetReachDirection(Right, Up);
		}
	}
	
	public void Grip(float right, float Up) {
		if (currentGripState != GripState.Gripped) {
			currentGripState = GripState.Gripping;
			reachDirection = SetReachDirection(Right, Up);
		}
	}
	
	public void Relax() {
		if (currentLimbState != LimbState.Relaxed) {
			currentLimbState = LimbState.Relaxed;
			reachDirection = new Vector3(0,0,0);

		}
	}
	
	public void Contract() {
		if (currentLimbState != LimbState.Contracting) {
			currentLimbState = LimbState.Contracting;
			//contract rotational limits
		}
	}
	
	public void Extend() {
		if (currentLimbState != LimbState.Extending) {
			currentLimbState = LimbState.Extending;
			//extend rotational limits
		}
	}	
}
