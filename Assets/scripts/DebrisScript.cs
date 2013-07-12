using UnityEngine;
using System.Collections;

// Controls the destruction of Debris

public class DebrisScript : MonoBehaviour {
	
	float lifeSpan = 10.0f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if( lifeSpan <= 0 ){
			Destroy(gameObject);
		}
		lifeSpan -= Time.deltaTime;
	}
	
	// If Collision with any entity tagged as "debrisDestroyer" remove debris after 5 seconds
	void OnCollisionEnter(Collision collision) {
		
		if(collision.gameObject.tag == "debrisDestroyer") {
			Destroy(gameObject,5.0f);			
		}
	}
}
