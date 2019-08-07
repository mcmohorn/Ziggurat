using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}


	void OnCollisionEnter(){
      Destroy (gameObject);
 }
	
	// Update is called once per frame
	void Update () {
		
	}
}
