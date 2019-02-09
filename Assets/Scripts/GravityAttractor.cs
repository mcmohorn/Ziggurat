using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAttractor : MonoBehaviour {
    public float gravity = -110f;

    public void Attract(Transform body) {

        Debug.Log("Attracted body");
        Vector3 targetDir = (body.position - transform.position).normalized;
        Vector3 bodyUp = body.up;

        body.rotation = Quaternion.FromToRotation(bodyUp, targetDir)*body.rotation;
        GetComponent<Rigidbody>().AddForce(targetDir * gravity);
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
