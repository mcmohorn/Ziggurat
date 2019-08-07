using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;

    public float liftSpeed;
    public float turnSpeed;

     public float bulletSpeed;

    public float fireRate;

    public float nextFire = 0f;

    private float left, right,leftH, rightH;
    private int collisionCount = 0;
    private Vector3 contactNormal;
    public GameObject bulletPrefab;
    public GameObject planetPrefab;

    public GameObject testCube;

    public float twistSpeed = 80f;


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

    public bool grounded = false;

    public float currentSpeed;
    public float jumpCooldown = 5;


    public float barDisplay; //current progress
     public Vector2 pos = new Vector2(20,40);
     public Vector2 size = new Vector2(60,20);
     public Texture2D emptyTex;
     public Texture2D fullTex;

     public float guiPadding; 

    private void Start()
    {
        Debug.Log("playform is " + Application.platform);
        threshold = Screen.height / 2;
        jumpCooldown = 0;


        pos = new Vector2(Screen.width - pos.x - size.x - guiPadding, Screen.height - pos.y - size.y - guiPadding);
        //Cursor.lockState = CursorLockMode.Locked;
         //   Cursor.lockState = CursorLockMode.None; 
        // CreatePlanets();

    }

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Hole1")
        {
            Physics.IgnoreCollision(other, GetComponent<Collider>(), true);
            Physics.IgnoreCollision(zig.GetComponent<Collider>(), GetComponent<Collider>(), true);
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Hole1")
        {
            Physics.IgnoreCollision(other, GetComponent<Collider>(), false);
            //Physics.IgnoreCollision(zig.GetComponent<Collider>(), GetComponent<Collider>(), false);

            // instant relocate
            //GetComponent<Rigidbody>().MovePosition(transform.position + transform.up*3);

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

    void OnGUI() {
        // STEP 1 : Health Bar
        //draw the background:
        GUI.BeginGroup(new Rect(pos.x, pos.y, size.x, size.y));
        GUI.Box(new Rect(0,0, size.x, size.y), emptyTex);
         
        //draw the filled-in part:
        GUI.BeginGroup(new Rect(0,0, size.x * barDisplay, size.y));
        GUI.Box(new Rect(0,0, size.x, size.y), fullTex);
        GUI.EndGroup();
        GUI.EndGroup();

        // STEP 2 : Speedometer
        // float speedBoxWidth = 30f;
        // GUI.BeginGroup(new Rect(guiPadding, Screen.height - guiPadding - speedBoxWidth, speedBoxWidth, speedBoxWidth));
        // GUI.Box(new Rect(0,0, speedBoxWidth, speedBoxWidth), fullTex);

        // Vector2 start = new Vector2(0, 0);
        // Vector2 finish = new Vector2(100, 100);
        // GUI.DrawLine(start, finish, Color.cyan, 2f);
        
        // GUI.Label(new Rect(0,0, speedBoxWidth, speedBoxWidth), "yo");
        // GUI.EndGroup();

        GUI.Box(new Rect(Screen.width/2,Screen.height/2, 10, 10), "");



     }
     
//      void Update() {
//          //for this example, the bar display is linked to the current time,
//          //however you would set this value based on your desired display
//          //eg, the loading progress, the player's health, or whatever.
         
//  //        barDisplay = MyControlScript.staticHealth;
//      }



    void Update()
    {

        barDisplay = 1.0f;

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
            leftH = Input.GetAxis("Horizontal1");
            rightH = Input.GetAxis("Horizontal2");

            var mousePos = Input.mousePosition;

            mousePos.x -= Screen.width/2;
            mousePos.y -= Screen.height/2;

            float maxX = Screen.width/2;
            float minX = 10f;

            if ((mousePos.x < maxX && mousePos.x > minX) || (mousePos.x > -maxX && mousePos.x < -minX) ) {
                rightH = -mousePos.x / maxX;
            }

            float maxY = Screen.width/2;
            float minY = 10f;

            if ((mousePos.y < maxY && mousePos.y > minY) || (mousePos.y > -maxY && mousePos.y < -minY) ) {
                right = -mousePos.y / maxY;
            }

            

            Debug.Log("Mouse is "+ mousePos);

            
            if (Input.GetKeyDown("joystick button 7") || Input.GetMouseButton(0))
            {
                if (Time.time > nextFire && fireRate > 0) {
                    nextFire = Time.time + fireRate;
                    Fire();
                }

            }
        }
    }

    void FixedUpdate()
    {
        if (collisionCount == 0)
        {
            grounded = false;
            //GetComponent<Rigidbody>().AddForce(Vector3.up * -9.8f, ForceMode.Acceleration);

        } else {
            grounded = true;
        }

        int i = 0;
        foreach (var planet in planets)
        {
            planet.transform.position = transform.position + planetPositions[i];
            i++;
        }

        // track current speed for grapple hook launch speed etc.
        currentSpeed = (left) * speed;
       // GetComponent<Rigidbody>().AddForce(transform.forward * left * speed, ForceMode.Force);
        // forward thrust
        GetComponent<Rigidbody>().AddForce(transform.forward * (left + 1f) * speed, ForceMode.Force);
        // left / right
        GetComponent<Rigidbody>().AddForce(transform.right * leftH * speed, ForceMode.Force);
        // torque
        GetComponent<Rigidbody>().AddTorque(transform.up * (-rightH) * turnSpeed, ForceMode.Force);  

         GetComponent<Rigidbody>().AddTorque(transform.right * (right) * turnSpeed, ForceMode.Force); 

        //Debug.Log("now its" + left);

        // scale the engines
        GameObject.Find("EngineMain").transform.localScale = new Vector3(left, left, left);
        //GameObject.Find("EngineLeft").transform.localScale = new Vector3(left, left, left);


        if(artificialGravityDirection.magnitude > 0 || this.GetComponent<GrappleHook>().hooked) {
            //this.GetComponent<Rigidbody>().useGravity = false;
        } else {
            //this.GetComponent<Rigidbody>().useGravity = true;
        }


        //if (grounded && currentSpeed < liftSpeed) GetComponent<Rigidbody>().AddForce(artificialGravityDirection * 9.8f, ForceMode.Acceleration);
        // 


        if (Input.GetKey("joystick button 3") || Input.GetKey(KeyCode.Space))
        {
            TrueFlatten();
            
        }



        if (Input.GetKey("joystick button 1"))
        {
            if (grounded) {
                AntiJump();
            } else {
                AntiDive();
            }
           
        }

        if (Input.GetKey("joystick button 4") || Input.GetKey("q") )
        {
            Flatten();
        }

        if (Input.GetKey("joystick button 5") || Input.GetKey("e"))
        {
            AntiFlatten();
        }

        // This would cast rays only against colliders in layer 8 .
    // var layerMask = 1<<8;
    // layerMask = ~layerMask;
    //    RaycastHit hit;
           
    // cast a ray to the right of the player object
    // if (Physics.Raycast (transform.position,transform.TransformDirection(-Vector3.up),out hit,30, layerMask)) {
        
    //     Debug.Log("hit " + hit.normal) ;
    //     // orient the Moving Object's Left direction to Match the Normals on his Right
    //     //var RunnerRotation = Quaternion.FromToRotation (Vector3.up, hit.normal);
    //      transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
    //      GetComponent<Rigidbody>().AddForce(-hit.normal * 9.8f, ForceMode.Acceleration);
        
    //     //Smooth rotation

    //     //transform.rotation = Quaternion.Slerp(transform.rotation, RunnerRotation,Time.deltaTime * 10);
    // }


    
    // control player bounds 
      if (transform.position.x > 1000f) {
          transform.SetPositionAndRotation(new Vector3(-500, transform.position.y,transform.position.z), transform.rotation);
      }
      if (transform.position.x < -500) {
          transform.SetPositionAndRotation(new Vector3(1000f, transform.position.y,transform.position.z), transform.rotation);
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
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * (bulletSpeed);
        Destroy(bullet, 3.0f);
    }

    void Flatten()
    {
         GetComponent<Rigidbody>().AddTorque(transform.forward * twistSpeed, ForceMode.Force);
        //transform.Rotate(Vector3.forward * Time.deltaTime * twistSpeed);
    }

    void TrueFlatten()
    {
        //var b = (GameObject)Instantiate(bulletPrefab, transform.position + transform.forward * 3.1f, transform.rotation);
        Vector3 direction = Vector3.Cross(transform.right, Vector3.up);
        

        //testCube.transform.localPosition = direction *3f;
        
        // Quaternion toRotation = Quaternion.FromToRotation(transform.forward, direction);
        // transform.localRotation = Quaternion.Lerp(transform.localRotation, toRotation, twistSpeed * Time.time);


        // The step size is equal to speed times frame time.
        float step = twistSpeed * Time.deltaTime;

        Vector3 newDir = Vector3.RotateTowards(transform.forward, direction, step, 0.0f);
        Debug.DrawRay(transform.position, newDir, Color.red);

        // Move our position a step closer to the target.
        transform.rotation = Quaternion.LookRotation(newDir);
    }

    void AntiFlatten()
    {
         GetComponent<Rigidbody>().AddTorque(-transform.forward * twistSpeed, ForceMode.Force);
        //transform.Rotate(-Vector3.forward * Time.deltaTime * twistSpeed);
    }

      void Dive()
    {

        transform.Rotate(Vector3.right * Time.deltaTime * twistSpeed);
    }

    void AntiDive()
    {
        transform.Rotate(-Vector3.right * Time.deltaTime * twistSpeed);
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

    void OnDrawGizmos()
    {
        Vector3 direction = Vector3.Cross(transform.right, Vector3.up);
        Gizmos.DrawLine(transform.position, transform.position+direction);
    }


}
