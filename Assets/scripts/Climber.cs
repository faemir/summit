using UnityEngine;
using System.Collections;

public class Climber : MonoBehaviour 
{
	
	public string[] toolStrings = new string[] {"CAM1", "CAM2"};
	
	private GameObject[] cameras;
	private int toolNum = 0;
	private int lastToolNum;
	private Transform head;
	private Vector3 startPos;
	private float maxdist;
	// Use this for initialization
	void Start () 
	{
		startPos = transform.position;
		head = transform.FindChild("Head");
		cameras = GameObject.FindGameObjectsWithTag("MainCamera");
		foreach (GameObject cam in cameras)
		{
			cam.SetActive(false);
			if (cam.name == "ChaseCam") 
			{
				cam.SetActive(true);
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	void OnGUI()
	{
		toolNum = GUI.Toolbar( new Rect(25,25,250,30), toolNum, toolStrings);
		
		if (toolNum != lastToolNum)
		{
			foreach (GameObject cam in cameras)
			{
				cam.SetActive(false);
			}
			cameras[toolNum].SetActive(true);
		}
		
		lastToolNum = toolNum;
		
		float distance = Vector3.Distance(startPos, head.transform.position);
		if (distance > maxdist) maxdist = distance;
		GUI.Label( new Rect(25, 60, 250, 30),"Best: " + maxdist);
	}
}
