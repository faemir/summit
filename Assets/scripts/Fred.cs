using UnityEngine;
using System.Collections;

public class Fred : MonoBehaviour 
{
	
	public enum Kinematic
	{
		None,
		Head,
		UpperTorso,
		BothHands,
		EntireBody,
	}
	
	public bool useGravity = true;
	public Kinematic MakeKinematic = Kinematic.None;
	public float Strength = 5f;
	
	private bool useGravityOld;
	private Kinematic MakeKinematicOld;
	private float OldStrength;
	// Use this for initialization
	void Start () 
	{
		SetGravity();
		SetKinematic();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (useGravity != useGravityOld)
			SetGravity();
		useGravityOld = useGravity;
		if (MakeKinematic != MakeKinematicOld)
			SetKinematic();
		MakeKinematicOld = MakeKinematic;
		if (Strength != OldStrength)
			SetStrength();
		OldStrength = Strength;
	}
	
	void SetGravity()
	{
		Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
		foreach (Rigidbody r in rigidbodies) 
		{
			r.useGravity = useGravity;
		}
	}
	
	void SetKinematic()
	{
		Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
		foreach (Rigidbody r in rigidbodies) 
		{
			r.isKinematic = false;
		}
		
		switch (MakeKinematic)
		{
		case Kinematic.None:
			break;
		case Kinematic.Head:
			foreach (Rigidbody r in rigidbodies)
			{
				if (r.transform.name == "Head")
					r.isKinematic = true;
			}
			break;
		case Kinematic.UpperTorso:
			foreach (Rigidbody r in rigidbodies)
			{
				if (r.transform.name == "UpperTorso")
					r.isKinematic = true;
			}
			break;
		case Kinematic.BothHands:
			foreach (Rigidbody r in rigidbodies)
			{
				if (r.transform.name == "RightHand" || r.transform.name == "LeftHand")
					r.isKinematic = true;
			}
			break;
		case Kinematic.EntireBody:
			foreach (Rigidbody r in rigidbodies)
			{
				r.isKinematic = true;
			}
			break;
		}

	}
	
	void SetStrength()
	{
		Limb[] limbs = GetComponentsInChildren<Limb>();
		foreach (Limb l in limbs)
		{
			l.maxReachForce = Strength;
		}
	}
}
