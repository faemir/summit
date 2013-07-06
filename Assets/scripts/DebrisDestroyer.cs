using UnityEngine;
using System.Collections;

public class DebrisDestroyer : MonoBehaviour 
{
	void OnCollisionEnter (Collision info)
	{
		if ( info.gameObject.tag == "debris" )
			Destroy (info.gameObject, 5f);
	}
}
