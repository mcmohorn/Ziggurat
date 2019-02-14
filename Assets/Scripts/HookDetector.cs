using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookDetector : MonoBehaviour {
	public GameObject player;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision collision){
    ContactPoint contact = collision.contacts[0];
	Debug.Log("Hook hit something");
		if (collision.gameObject.tag == "Hookable") {

			Debug.Log("Hook hit something and its hookable" + collision.contacts[0].normal);
			player.GetComponent<GrappleHook>().hooked = true;
			player.GetComponent<GrappleHook>().hookPosition = transform.position;
			player.GetComponent<GrappleHook>().hookedObject = collision.gameObject;
			player.GetComponent<GrappleHook>().hookedNormal = collision.contacts[0].normal;
		}
        //contact.point; //this is the Vector3 position of the point of contact
}

	// void OnTriggerEnter(Collider other) 
	// {
	// 	Debug.Log("Hook hit something");
	// 	if (other.tag == "Hookable") {

	// 		Debug.Log("Hook hit something and its hookable");
	// 		player.GetComponent<GrappleHook>().hookPosition = transform.position;
	// 		player.GetComponent<GrappleHook>().hooked = true;
	// 		player.GetComponent<GrappleHook>().hookedObject = other.gameObject;
	// 	}
	// }
}
