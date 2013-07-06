using UnityEngine;
using System.Collections;

public class CameraFollowClimber : MonoBehaviour 
{
	
	public Transform climber;
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 targetpos;
		targetpos.x = climber.transform.position.x * 2;
		targetpos.y = Mathf.Abs(climber.position.y - 2f);
		targetpos.z = climber.position.z * 2;
		if (targetpos.x >= 18f) targetpos.x = 18f;
		if (targetpos.x <= -18f) targetpos.x = -18f;
		if (targetpos.z >= 18f) targetpos.z = 18f;
		if (targetpos.z <= -18f) targetpos.z = -18f;
		transform.position = targetpos;
		transform.LookAt(climber.position + Vector3.up * 2f);
	}
}
