using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    public static bool fired = false;
    public  bool hooked = false;
    public float maxDistance;
    public float minDistance;
    public float hookTravelSpped = 10.0f;
    public float playerTravelSpeed = 10.0f;
    private float currentDistance;

    public Vector3 hookPosition;
    public Vector3 hookedNormal;

    public GameObject hook;
    public GameObject hookHolder;
    public GameObject hookTarget;

    public GameObject hookedObject;


    void Start()
    {
        hook.transform.position = hookHolder.transform.position;
        hook.transform.rotation = hookHolder.transform.rotation; 
    }

    void Update()
    {
        
        if (Input.GetKeyDown("joystick button 4") && fired == false)
        {
            fired = true;
        }
        if (fired == true && hooked == false)
        {
            float playerCurrentSpeed = this.GetComponent<PlayerController>().currentSpeed;
            hook.transform.position = Vector3.MoveTowards(hook.transform.position, hookTarget.transform.position,  Time.deltaTime * (hookTravelSpped + playerCurrentSpeed));
			currentDistance = Vector3.Distance(hookHolder.transform.position, hook.transform.position);
            if (currentDistance > maxDistance) ReturnHook();
        } else if (fired == false) {
            hook.transform.position = hookHolder.transform.position;
            hook.transform.rotation = hookHolder.transform.rotation;
        }
        
        if (hooked == true) {
            // keep updating hooks position as still so it is set relative to the player which is moving at each step
            hook.transform.position = hookPosition;
            // move the player towards the hook
            transform.position = Vector3.MoveTowards(transform.position, hook.transform.position, Time.deltaTime * playerTravelSpeed);
            currentDistance = Vector3.Distance(hookHolder.transform.position, hook.transform.position);
            //ignore gravity when hooked
            this.GetComponent<Rigidbody>().useGravity = false;
            
            // release the hook
            if (currentDistance < minDistance){
                // orient the player with the new surface
                transform.up = hookedNormal;
                this.GetComponent<PlayerController>().artificialGravityDirection = -1.0f*hookedNormal;
                transform.position = transform.position + transform.forward*2.1f - 1.5f*transform.up;
                ReturnHook();
            } 
            // also release hook if we get too far away
            if (currentDistance > maxDistance) {
                ReturnHook();
            }
        } else {
            
            hook.transform.parent = hookHolder.transform; // TODO can we remove this line
            // this.GetComponent<Rigidbody>().useGravity = true;
        }
        
    }

    void ReturnHook()
    {
        Debug.Log("returning hook");
		hook.transform.position = hookHolder.transform.position;
        hook.transform.rotation = hookHolder.transform.rotation; // this adds a 90 degrees Y rotation
		fired = false;
		hooked = false;
    }
}
