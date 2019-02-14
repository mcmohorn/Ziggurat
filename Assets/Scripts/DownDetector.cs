using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownDetector : MonoBehaviour {
public GameObject player;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnTriggerEnter(Collider other) 
	{
		//Debug.Log("down thing is touching !!!");
		if (other.tag == "Hookable") {

			// Debug.Log("Hook hit something and its hookable");
			// player.GetComponent<GrappleHook>().hookPosition = transform.position;
			// player.GetComponent<GrappleHook>().hooked = true;
			// player.GetComponent<GrappleHook>().hookedObject = other.gameObject;
		}
	}

	void OnTriggerExit(Collider other) 
	{
		//Debug.Log("no longer touching" + other.gameObject.name);
		if (other.tag == "Hookable") {
			
			//player.transform.up = transform.up;
			Debug.Log("no longer touching the hookable object " + other.gameObject.name);
			player.transform.position = player.transform.position + 1.5f*player.transform.forward;

			player.transform.rotation = transform.rotation;
			// player.transform.forward = transform.up;
			player.transform.Rotate(0,90,0);

			// player.transform.localRotation= Quaternion.Euler(0,0,-90f);
			player.GetComponent<PlayerController>().artificialGravityDirection = -1.0f*player.transform.up;

			// Debug.Log("Hook hit something and its hookable");
			// player.GetComponent<GrappleHook>().hookPosition = transform.position;
			// player.GetComponent<GrappleHook>().hooked = true;
			// player.GetComponent<GrappleHook>().hookedObject = other.gameObject;
		}
	}
}
