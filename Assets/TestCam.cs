using UnityEngine;
using System.Collections;

public class TestCam : MonoBehaviour 
{
	public float moveSpeed = 5f;
	public float rotateSpeed = 5f;
	
	// Use this for initialization
	void Start () 
	{
		Screen.lockCursor = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		if (Input.GetKey(KeyCode.W))
			transform.position += transform.forward * Time.deltaTime * moveSpeed;
		if (Input.GetKey(KeyCode.A))
			transform.position -= transform.right * Time.deltaTime * moveSpeed;
		if (Input.GetKey(KeyCode.S))
			transform.position -= transform.forward * Time.deltaTime * moveSpeed;
		if (Input.GetKey(KeyCode.D))
			transform.position += transform.right * Time.deltaTime * moveSpeed;
		
		float rotX = Input.GetAxis("Mouse X") * Time.deltaTime * rotateSpeed;
		float rotY = Input.GetAxis("Mouse Y") * Time.deltaTime * rotateSpeed;
		transform.RotateAround(Vector3.up, rotX);
		transform.RotateAround(transform.right, -rotY);
		
	}
	
	void OnTriggerStay(Collider info)
	{
		if (info.tag == "hold")
		{
			float dist = Vector3.Magnitude(transform.position -  info.transform.position);
			float maxdist = GetComponent<SphereCollider>().radius;
			info.renderer.material.color = Color.Lerp(Color.white, Color.magenta, (maxdist-dist)/maxdist);
		}
	}
}
