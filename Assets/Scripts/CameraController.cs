using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject player;

    private float offsetScale = 5.0f;


	// LateUpdate is called once per frame, guaranteed to run after all items processed in update
	// so we know the player has already moved for this frame

	void LateUpdate () {

        if (Input.GetKey("joystick button 5") )
        {
            // look backwards
            //transform.position = player.transform.position + transform.up;

            //transform.LookAt(player.transform.position + transform.up - offsetScale * player.transform.forward);
        } else {
            // look forwards, keep camera above the player
            transform.position = player.transform.position + player.transform.up - 1.0f * player.transform.forward;

            transform.rotation = player.transform.rotation;
            //transform.rotation = Quaternion.Euler(0,0,0);

            //transform.LookAt(player.transform.position + player.transform.up);

            //transform.position = player.transform.position + transform.up - offsetScale * player.transform.forward;

            //transform.LookAt(player.transform.position + transform.up);
        }


	}

}
