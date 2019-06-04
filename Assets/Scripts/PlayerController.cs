using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float turnSpeed;

    public float shipRadius;
    public float radius;
    private float left, right;
    private int collisionCount = 0;
    private Vector3 contactNormal;
    public GameObject bulletPrefab;
    public GameObject planetPrefab;
    public GameObject holePrefab;
    public GameObject rightEngine;

    public GameObject zig;
    public Transform bulletSpawn;
    public float jumpForce;
    private int rightFingerId = -1;
    private int leftFingerId = -1;
    private Vector2 rightFingerOrigin;
    private Vector2 leftFingerOrigin;

    private List<Vector3> planetPositions;
    private List<Vector3> planetSizes;

    public List<GameObject> planets;

    public Vector3 artificialGravityDirection;

    private float s = 0;
    public float threshold;

    public float currentSpeed;
    public float jumpCooldown = 5;

    private void Start()
    {
        Debug.Log("playform is " + Application.platform);
        threshold = Screen.height / 2;
        jumpCooldown = 0;

        CreatePlanets();

        CreateHoles();

        //
    }

    private void CreateHoles()
    {
        //var hole = (GameObject)Instantiate(holePrefab, new Vector3(10,0,0), new Quaternion(0,0,0,0));

        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.gameObject.name = "Hole1";
        sphere.gameObject.transform.position = new Vector3(10, 0, 0);
        sphere.gameObject.transform.localScale = new Vector3(10, 10, 10);
        sphere.gameObject.layer = 8;
        sphere.gameObject.AddComponent<Rigidbody>();
        sphere.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        sphere.gameObject.GetComponent<Rigidbody>().useGravity = false;

        var sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.radius = 1.5f;
        sphereCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Hole1")
        {
            Debug.Log("trigger entered, collider name: " + other.name + " gon:" + other.gameObject.name);
            Physics.IgnoreCollision(other, GetComponent<Collider>(), true);
            Physics.IgnoreCollision(zig.GetComponent<Collider>(), GetComponent<Collider>(), true);
        }
        //{
        // Debug.Log("trigger entered, collider name: " + other.name + " gon:" + other.gameObject.name);
        //}
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Hole1")
        {
            Physics.IgnoreCollision(other, GetComponent<Collider>(), false);
            Physics.IgnoreCollision(zig.GetComponent<Collider>(), GetComponent<Collider>(), false);

        }
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

        int i = 0;
        foreach (var pos in planetPositions)
        {
            var planet = (GameObject)Instantiate(planetPrefab, transform.position + pos, new Quaternion(0, 0, 0, 0));
            planet.transform.localScale = planetSizes[i];
            planets.Add(planet);
            i++;
        }

    }

    void UpdatePlanets()
    {
        /*
        use an outwards raycast to determine if planet should be hidden from the user (away from camera)
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            Debug.Log("Did not Hit");
        }
        */
    }



    void Update()
    {


        bool doMobile = false;

        if (jumpCooldown > 0)
        {
            jumpCooldown -= Time.deltaTime;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer || doMobile)
        {
            UpdateMobile();
        }
        else
        {

            left = Input.GetAxis("Vertical1");
            right = Input.GetAxis("Vertical2");
            if (Input.GetKeyDown("joystick button 7") || Input.GetKeyDown(KeyCode.Space))
            {
                Fire();

            }

            {

            }

        }

    }

    void FixedUpdate()
    {
        if (collisionCount == 0)
        {
            Debug.Log("no collisions !!!!!!");
        }
        // Debug.Log("left  is " + left + "right is " + right);
        int i = 0;
        foreach (var planet in planets)
        {
            planet.transform.position = transform.position + planetPositions[i];
            i++;
        }

        float movement = (left + right) * speed;
        s = movement;
        currentSpeed = movement;

        float mass = GetComponent<Rigidbody>().mass;
        // add forward thrust

        // float rightEngine = 1.0f;

        //GetComponent<Rigidbody>().AddForceAtPosition(transform.forward * left * speed, transform.position - transform.right * shipRadius);
        //GetComponent<Rigidbody>().AddForceAtPosition(transform.forward * right * speed, transform.position + transform.right * shipRadius);
        
        // forward
        GetComponent<Rigidbody>().AddForce(transform.forward * (left+right) * speed, ForceMode.Acceleration);
        // torque
        GetComponent<Rigidbody>().AddTorque(transform.up * (left-right) * turnSpeed, ForceMode.Acceleration);

    

        


        // TODO: this single statement is equivalent to the following two, I am  trying to figure out why the turning directions behave differently
        //float torque = (left - right) * turnSpeed * Time.deltaTime;
        //GetComponent<Rigidbody>().AddTorque(transform.up * torque, ForceMode.Acceleration);
        //GetComponent<Rigidbody>().AddTorque(transform.up * right * turnSpeed * Time.deltaTime, ForceMode.VelocityChange);
        //GetComponent<Rigidbody>().AddTorque(-1.0f * transform.up * left * turnSpeed * Time.deltaTime, ForceMode.VelocityChange);
        //  Debug.Log("torque" + torque);

        if(artificialGravityDirection.magnitude > 0 || this.GetComponent<GrappleHook>().hooked) {
            //this.GetComponent<Rigidbody>().useGravity = false;
        } else {
            //this.GetComponent<Rigidbody>().useGravity = true;
        }
        // GetComponent<Rigidbody>().AddForce(artificialGravityDirection * 9.8f, ForceMode.Acceleration);

        if (Input.GetKeyDown("joystick button 3"))
        {
            Jump();
        }

        if (Input.GetKeyDown("joystick button 1"))
        {
            AntiJump();
        }

        if (Input.GetKey("joystick button 0"))
        {
            Flatten();
        }





    }

    void OnCollisionEnter(Collision col)
    {
        collisionCount++;

        print("Collided with: " + col.gameObject.name + " at contact point " + col.contacts[0].normal);

        if (col.gameObject.name == "Terrain2")
        {
            jumpCooldown = 0;
        }

    }




    // TODO: Jump cooldown solution?
    void Jump()
    {
        ///if (jumpCooldown <= 0) {
        GetComponent<Rigidbody>().AddForce(transform.up * jumpForce, ForceMode.Impulse);
        // jumpCooldown = 5;
        //} else {
        //   Debug.Log("seconds left until jump available " + jumpCooldown);
        // }
    }
    void AntiJump()
    {
        // if (jumpCooldown <= 0) {
        GetComponent<Rigidbody>().AddForce(-1.0f * transform.up * jumpForce, ForceMode.Impulse);
        //  jumpCooldown = 5;
        //  } else {
        //  Debug.Log("seconds left until anti-jump available " + jumpCooldown);
        //  }

    }

    void Fire()
    {
        var bullet = (GameObject)Instantiate(bulletPrefab, transform.position + transform.forward * 3.1f, transform.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * (200f);
        Destroy(bullet, 3.0f);
    }

    void Flatten()
    {
        Debug.Log("twisting...");
        // Rotate our transform a step closer to the target's.
        //var rot = Quaternion.FromToRotation(-transform.right, transform.up);
        //Vector3 diff =  Vector3.up - transform.up;
        //Debug.Log("Diff is " + diff);
        //if (diff.magnitude > 0.01f) {

        transform.Rotate(Vector3.forward * Time.deltaTime * 40f);


        //}
        //transform.localRotation = Quaternion.RotateTowards(transform.rotation, rot, Time.deltaTime * 40.0f);
        //transform.localRotation = rot;

        //transform.rotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
    }



    private void OnCollisionExit(Collision col)
    {

        collisionCount--;
    }


















    void UpdateMobile()
    {
        // WORK IN PROGRESS
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
    }


}
