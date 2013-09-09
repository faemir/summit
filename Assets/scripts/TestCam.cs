using UnityEngine;
using System.Collections;

public class TestCam : MonoBehaviour 
{
	public bool disableMovement = true;
	public bool disableRaycast = true;
	public float cameraMoveSpeed = 5f;
	public float cameraRotateSpeed = 5f;
	
	public Color farHoldColor = Color.red;
	public Color nearHoldColor = Color.green;
	
	// percentage of minimum distance for color
	public float nearThreshold = 0.25f;
	
	// Use this for initialization
	void Start () 
	{
		//Screen.lockCursor = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!disableMovement)
		{
			float moveSpeed = cameraMoveSpeed;
			if (Input.GetKey(KeyCode.LeftShift))
				moveSpeed *= 2f;
			
			if (Input.GetKey(KeyCode.W))
				transform.position += transform.forward * Time.deltaTime * moveSpeed;
			if (Input.GetKey(KeyCode.A))
				transform.position -= transform.right * Time.deltaTime * moveSpeed;
			if (Input.GetKey(KeyCode.S))
				transform.position -= transform.forward * Time.deltaTime * moveSpeed;
			if (Input.GetKey(KeyCode.D))
				transform.position += transform.right * Time.deltaTime * moveSpeed;
			if (Input.GetKey(KeyCode.Q))
				transform.position -= transform.up * Time.deltaTime * moveSpeed;
			if (Input.GetKey(KeyCode.E))
				transform.position += transform.up * Time.deltaTime * moveSpeed;
			
			float rotX = Input.GetAxis("Mouse X") * Time.deltaTime * cameraRotateSpeed;
			float rotY = Input.GetAxis("Mouse Y") * Time.deltaTime * cameraRotateSpeed;
			
			transform.Rotate(Vector3.up, rotX, Space.World);
			transform.Rotate(Vector3.right, -rotY, Space.Self);
		}
		
		if (Input.GetMouseButton(0) && !disableRaycast)
		{
			Ray cast = new Ray(transform.position, transform.forward);
			RaycastHit hit;
			if (Physics.Raycast(cast, out hit))
			{
				Debug.DrawRay(hit.point,hit.normal);
				Debug.DrawLine(transform.position, hit.point);
				if ( hit.transform.tag == "hold" )
				{
					ScoreManager.UseHold(hit.transform);
				}
			}
			
		}
		
	}
	
	void OnTriggerStay(Collider info)
	{
		if (info.tag == "hold")
		{
			float dist = Vector3.Magnitude(transform.position -  info.transform.position);
			float maxdist = GetComponent<SphereCollider>().radius;
			Color c = Color.white;
			
			if (dist <= nearThreshold * maxdist)
			{
				//Debug.DrawLine(transform.position, info.transform.position, Color.green);
				c = Color.Lerp(nearHoldColor, farHoldColor, dist / (nearThreshold * maxdist));
			}
			if (dist > nearThreshold * maxdist)
			{
				//Debug.DrawLine(transform.position, info.transform.position, Color.red);
				c = Color.Lerp(farHoldColor, Color.white, dist / maxdist);
			}
			ScoreManager.SetHoldColour(info.transform, c);
		}
	}
}
