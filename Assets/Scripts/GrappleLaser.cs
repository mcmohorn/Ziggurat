using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleLaser : MonoBehaviour {
	LineRenderer line;
	// Use this for initialization
	void Start () {
		line = gameObject.GetComponent<LineRenderer>();
		line.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Grapple")) {
			StopCoroutine("FireLaser");
			StartCoroutine("FireLaser");
		}
	}

	IEnumerator FireLaser()
	{
		line.enabled = true;
		while(Input.GetButton("Grapple")) {
			//Debug.Log("Grappling");
			//line.material.mainTextureOffset = new Vector2(0, Time.time);
			//line.renderer.material.mainTextureOffset = 
			Ray ray = new Ray(transform.position, transform.forward);
			RaycastHit hit;

			line.SetPosition(0,ray.origin);
			line.SetPosition(1,ray.origin + transform.forward);
			if (Physics.Raycast(ray, out hit, 100)) {
				line.SetPosition(1, hit.point);
			} else {
				line.SetPosition(1, ray.GetPoint(100));
			}
			yield return null;
		}
		 line.enabled= false;
	}
}
