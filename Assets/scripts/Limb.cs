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
	
	public GripState currentGripState = GripState.LetGo;
	
	// Use this for initialization
	void Start () {
		
	}
	
	void fixedUpdate () {
		//physics actions
	}
	
	public void Grip () {
		currentGripState = GripState.Gripping;
	}
	
	public void Reach () {
		currentGripState = GripState.Reaching;
	}
	
	public void Pull () {
		if (currentGripState == GripState.Gripped) {
			
		}
	}
	
	public void Push () {
		if (currentGripState == GripState.Gripped) {
			
		}
	}
	
	
}
