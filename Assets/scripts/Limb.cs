using UnityEngine;
using System.Collections;

public class Limb : MonoBehaviour {
	
	public enum GripState {
		LetGo,
		Reaching,
		Gripping,
		Gripped
		
	}
	
	public GripState gripState = GripState.LetGo;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void Grip () {
		
	}
	
	public void Reach () {
		
	}
	
	public void Pull () {
		
	}
	
	public void Push () {
		
	}
}
