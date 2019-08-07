using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {
    Material m_material;
    public float speed;

    public GameObject zig;
    private Transform target = null;
    void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player") {
            target = other.transform;
            m_material.color = Color.red;

        } 
    }
    

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player") {
            target = null;
            Debug.Log("player exitted collision box");
            m_material.color = Color.black;

           
        } 
    }

	// Use this for initialization
	void Start () {
        Debug.Log("Enemy controller started");
        m_material = GetComponent<Renderer>().material;
        print("Materials " + Resources.FindObjectsOfTypeAll(typeof(Material)).Length);
        Physics.IgnoreCollision(zig.GetComponent<Collider>(), GetComponent<Collider>(), true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}



    void FixedUpdate()
    {
        if (target) {
            Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, target.position.z);
            transform.rotation = Quaternion.LookRotation(targetPosition - transform.position);
            GetComponent<Rigidbody>().AddForce(transform.forward * speed *2.0f, ForceMode.Acceleration);
        }
        //Collider[] thingsToHit = UnityEngine.Physics.OverlapSphere(centerOfEx, explosionRadius, effectedLayers);
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        //Ray rayToCameraPos = new Ray(transform.position, hit.transform.position - transform.position);
        /*
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, 5)) {
            if (hitInfo.collider.tag == hit.tag)
            {
                Debug.Log(hit.name + " TRUE");
                m_material.color = Color.red;

                if (hit.GetComponent<Rigidbody>() != null && hit.gameObject.GetComponent<LandMine>() == null)
                {
                    hit.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, centerOfEx, explosionRadius, 1, ForceMode.Impulse);
                }
                float proximity = (centerOfEx - hit.transform.position).magnitude;
                float effect = 1 - (proximity / explosionRadius);

                if (hit.gameObject.GetComponent<LandMine>() != null)
                {
                    hit.gameObject.GetComponent<LandMine>().time = 0.25f;
                }

                hit.gameObject.SendMessage("ApplyDamage", damage * effect, SendMessageOptions.DontRequireReceiver);

            }

        }
        else {
            m_material.color = Color.black;
        }
           */
            
    }
}
