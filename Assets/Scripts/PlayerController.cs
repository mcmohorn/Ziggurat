using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public float speed;
	public float turnSpeed;
    private float left, right;
    private int collisionCount = 0;
    private Vector3 contactNormal;
    public GameObject bulletPrefab;
    public GameObject planetPrefab;
    public GameObject rightEngine;
    public Transform bulletSpawn;
    public float jumpForce;
    private int rightFingerId = -1;
    private int leftFingerId = -1;
    private Vector2 rightFingerOrigin;
    private Vector2 leftFingerOrigin;

    private List<Vector3> planetPositions;
    private List<Vector3> planetSizes;

    public List<GameObject>planets;

    private float s = 0;
    public float threshold;
    public float jumpCooldown = 5;

	private void Start()
	{
        Debug.Log("playform is " + Application.platform);
        threshold = Screen.height / 2;
        jumpCooldown = 0;
        
        CreatePlanets();
        

	}

    private void CreatePlanets()
    {
        planetPositions = new List<Vector3>(10);
        planetSizes = new List<Vector3>(10);

        planetPositions.Add(new Vector3(0, 20, -200));
        planetPositions.Add(new Vector3(100, 10, -150));
        planetPositions.Add(new Vector3(0, 30, -150));
        planetPositions.Add(new Vector3(80, 20, 200));
        planetPositions.Add(new Vector3(200, 20, 150));

        planetSizes.Add(new Vector3(2.0f, 2.0f, 2.0f));
        planetSizes.Add(new Vector3(2.0f, 2.0f, 2.0f));
        planetSizes.Add(new Vector3(2.0f, 2.0f, 2.0f));
        planetSizes.Add(new Vector3(1.5f, 1.5f, 1.5f));
        planetSizes.Add(new Vector3(1.0f, 1.0f, 1.0f));

        int i=0;
        foreach( var pos in planetPositions) {
            var planet = (GameObject)Instantiate(planetPrefab, transform.position + pos, new Quaternion(0,0,0,0));
            planet.transform.localScale = planetSizes[i];
            planets.Add(planet);
            i++;
        }
    }

	void Update()
	{
        bool doMobile = false;

        if (jumpCooldown > 0)
        {
            jumpCooldown -= Time.deltaTime;
        }
        if (Application.platform == RuntimePlatform.IPhonePlayer || doMobile) {
            // TODO only do this if platform is mobile
            foreach (var touch in Input.touches)
            {

                if (touch.phase == TouchPhase.Began)
                {
                    Debug.Log("TOUCH BEGAN" + touch.fingerId + ",");


                    if ((touch.position.x > Screen.width / 2f) && (touch.position.y > Screen.height / 2f) && rightFingerId == -1)
                    {
                        //new right finger origin on right of screen
                        right = 1;
                        rightFingerId = touch.fingerId;



                    }
                    else if ((touch.position.x > Screen.width / 2f) && (touch.position.y <= Screen.height / 2f) && rightFingerId == -1)
                    {
                        rightFingerId = touch.fingerId;
                        right = -1;
                    }
                    else if ((touch.position.x < Screen.width / 2f) && (touch.position.y > Screen.height / 2f) && rightFingerId == -1)
                    {
                        //new left finger at top
                        left = 1;
                        leftFingerId = touch.fingerId;
                        //rightFingerOrigin = touch.position;



                    }
                    else if ((touch.position.x < Screen.width / 2f) && (touch.position.y <= Screen.height / 2f) && rightFingerId == -1)
                    {
                        leftFingerId = touch.fingerId;
                        left = -1;
                    }


                }
                if (touch.phase == TouchPhase.Moved)
                {
                    Debug.Log("TOUCH MOVED" + touch.fingerId + ",");
                    if (touch.fingerId == rightFingerId)
                    {
                        //right = (touch.position.y - Screen.height / 2f) / (Screen.height / 2f);
                        //float diffY = 0;
                        //if (touch.position.y >= rightFingerOrigin.y) {
                        //    Debug.Log("TOUCH Moved UP" + touch.fingerId);
                        //    diffY = Mathf.Min(touch.position.y - rightFingerOrigin.y, threshold);
                        //} else {
                        //    Debug.Log("TOUCH Moved Down" + touch.fingerId);
                        //    diffY = Mathf.Max(touch.position.y - rightFingerOrigin.y, -1f*threshold);
                        //}
                        //right = diffY / (threshold);
                    }

                    if (touch.fingerId == leftFingerId)
                    {
                        //left = (touch.position.y - Screen.height / 2f) / (Screen.height / 2f);
                        //float diffY = 0;
                        //if (touch.position.y >= leftFingerOrigin.y)
                        //{   
                        //    diffY = Mathf.Min(touch.position.y - leftFingerOrigin.y, threshold);
                        //}
                        //else
                        //{

                        //    diffY = Mathf.Max(touch.position.y - leftFingerOrigin.y, -1f * threshold);
                        //}
                        //left = diffY / (Screen.width / 4f);
                    }
                    //Debug.Log("Left" + left);
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    Debug.Log("TOUCH ENDED " + touch.fingerId);
                    if (touch.fingerId == rightFingerId)
                    {
                        right = 0;
                        rightFingerId = -1;
                    }
                    if (touch.fingerId == leftFingerId)
                    {
                        left = 0;
                        leftFingerId = -1;
                    }
                }

                if (touch.phase == TouchPhase.Canceled)
                {
                    Debug.Log("TOUCH CANCELLED " + touch.fingerId);
                    if (touch.fingerId == rightFingerId)
                    {
                        right = 0;
                        rightFingerId = -1;
                    }
                    if (touch.fingerId == leftFingerId)
                    {
                        left = 0;
                        leftFingerId = -1;
                    }
                }
            }  
        } else {
            left = Input.GetAxis("Vertical2");
            right = Input.GetAxis("Vertical1");
            if (Input.GetKeyDown("joystick button 7") || Input.GetKeyDown(KeyCode.Space))
            {
                Fire();

            }
           
        }
           


       



	}

	void FixedUpdate () 
    {

        // Debug.Log("left  is " + left + "right is " + right);
        int i =0;
        foreach (var planet in planets) {
            planet.transform.position = transform.position + planetPositions[i];
            i++;
        }

        float movement = (left + right) * speed;
        s = movement;


        //float engineMag = Mathf.Clamp01(movement);
       // irightEngine.transform.localScale = new Vector3(engineMag, engineMag, engineMag);

        GetComponent<Rigidbody>().AddForce(transform.forward * movement,ForceMode.Acceleration);


        //float torque = (left - right) * turnSpeed * Time.deltaTime;
        //GetComponent<Rigidbody>().AddTorque(transform.up * torque, ForceMode.VelocityChange);

        GetComponent<Rigidbody>().AddTorque(transform.up * right * turnSpeed * Time.deltaTime, ForceMode.VelocityChange);
        GetComponent<Rigidbody>().AddTorque(-1.0f * transform.up * left * turnSpeed * Time.deltaTime, ForceMode.VelocityChange);

        if (Input.GetKeyDown("joystick button 1"))
        {
            Jump();
        }



	}

    void OnCollisionEnter(Collision col)
    {
        collisionCount++;
        
        print("First normal of the point that collide: " + col.contacts[0].normal);
        if (col.gameObject.name == "Terrain2" )
        {
            print("Hit Terrain  " );
            jumpCooldown = 0;


            if (col.contacts[0].normal == Vector3.up) {
                print("WE UPSIDE DOWN NOW "); 
            }
           //GetComponent<Rigidbody>().AddForce(col.contacts[0].normal * 9.8f);
            //var rot = Quaternion.FromToRotation(transform.up, col.contacts[0].normal);
            //Vector3 n = col.contacts[0].normal;
            //Vector3 d = transform.forward;
            //Vector3 dir = Vector3.Cross(n, d).normalized;

            //transform.rotation = Quaternion.FromToRotation(transform.forward, dir) * transform.rotation;

        } else {
            
        }

        // 
        if (col.gameObject.name == "Cube") {
            Destroy(col.gameObject);
        }
    }

    void Fire()
    {
        
        // Create the Bullet from the Bullet Prefab
        var bullet = (GameObject)Instantiate(
            bulletPrefab,
            transform.position + transform.forward*3.1f,
            transform.rotation);

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * (200f);

        // Destroy the bullet after 2 seconds
        Destroy(bullet, 3.0f);
    }

    void Jump()
    {
        if (jumpCooldown <= 0) {
            GetComponent<Rigidbody>().AddForce(transform.up * jumpForce, ForceMode.Impulse);
            Debug.Log("jump force " + transform.up * jumpForce);
            jumpCooldown = 5;
        } else {
            Debug.Log("seconds left until jump available " + jumpCooldown);
        }

       
    }

	private void OnCollisionExit(Collision col)
	{
       

	}


}
