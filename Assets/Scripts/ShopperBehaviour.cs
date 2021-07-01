using UnityEngine;
using System.Collections;

namespace UnityMovementAI
{
    public class ShopperBehaviour : MonoBehaviour
    {

        //public Vector3 targetPosition = new Vector3();


        public GameObject targetObject;
        private Vector3 targetPosition = new Vector3();
        private float maxVelocity = 10f;

        private float maxAcceleration = 10f;

        private float turnSpeed = 20f;

        private Rigidbody rb;

        private float targetRadius = 0.005f;

        private float slowRadius = 50f;

        public float timeToTarget = 0.1f;

        private Vector3 moveVector;

    //
        private float  rayLength = 10.0f;
        private float lerpSpeed = 1.0f;

        private float mult = 1.0f;
        private float speed = 20.0f;

        private static Vector3[] rayArray;
        private Vector3 lerpedTargetDir;


        private bool goingShop;
        private bool goingFood;
        private bool initalDecision = false;

        private GameObject[] shopEntrances;
        private GameObject[] chairs;

        private GameObject ShopsUp;
        private GameObject ShopsDown;

        private bool insideStoreAlready;

        void Start()
        {
            
            initalDecision = false;
            shopEntrances = GameObject.FindGameObjectsWithTag("ShopEntrance");
            chairs = GameObject.FindGameObjectsWithTag("Chair");

            ShopsUp = GameObject.Find("/ShopsUp");
            ShopsDown = GameObject.Find("/ShopsDown");


            //targetObject = GameObject.Find("MallExit");
            //targetPosition = targetObject.transform.position;
            rb = GetComponent<Rigidbody>();
            //steeringBasics = GetComponent<SteeringLogic>();
            //Vector3 accel = Arrive(targetPosition);

            rayArray = new Vector3[3];

            
        }
        void FixedUpdate()
        {
            //targetPosition = targetObject.transform.position;
            //Vector3 accel = Arrive(targetPosition);

            //Steer(targetPosition, accel);
            //avoidObstacles();
            //LookWhereYoureGoing();
            if (!initalDecision){

                if (Random.value > 0.5f){
                    goingShop = false;
                    targetObject = GameObject.Find("MallExit");
                    initalDecision = true;
                }
                else {
                    goingShop = true;
                    targetObject = shopEntrances[Random.Range(0,shopEntrances.Length-1)];
                    //targetObject = GameObject.Find("ShopEntrance");
                    initalDecision = true;
                }
            }
            else {
                if (goingFood) {
                    targetObject = chairs[Random.Range(0,chairs.Length-1)];
                }
            }
            //Debug.Log(shopEntrances.Length);
            moveAndAvoid(targetObject);            
        }
        public Vector3 Arrive(Vector3 targetPosition)
        {
            Debug.DrawLine(transform.position, targetPosition, Color.cyan, 0f, false);

            //targetPosition = rb.ConvertVector(targetPosition);

            /* Get the right direction for the linear acceleration */
            Vector3 targetVelocity = targetPosition - rb.position;
            //Debug.Log("Displacement " + targetVelocity.ToString("f4"));

            /* Get the distance to the target */
            float dist = targetVelocity.magnitude;

            /* If we are within the stopping radius then stop */
            if (dist < targetRadius)
            {
                rb.velocity = Vector3.zero;
                return Vector3.zero;
            }

            /* Calculate the target speed, full speed at slowRadius distance and 0 speed at 0 distance */
            float targetSpeed;
            if (dist > slowRadius)
            {
                targetSpeed = maxVelocity;
            }
            else
            {
                targetSpeed = maxVelocity * (dist / slowRadius);
            }

            /* Give targetVelocity the correct speed */
            targetVelocity.Normalize();
            targetVelocity *= targetSpeed;

            /* Calculate the linear acceleration we want */
            Vector3 acceleration = targetVelocity - rb.velocity;
            /* Rather than accelerate the character to the correct speed in 1 second, 
             * accelerate so we reach the desired speed in timeToTarget seconds 
             * (if we were to actually accelerate for the full timeToTarget seconds). */
            acceleration *= 1 / timeToTarget;

            /* Make sure we are accelerating at max acceleration */
            if (acceleration.magnitude > maxAcceleration)
            {
                acceleration.Normalize();
                acceleration *= maxAcceleration;
            }
            //Debug.Log("Accel " + acceleration.ToString("f4"));
            return acceleration;
        }

        void avoidObstacles()
        {
        Vector3 ahead = transform.right;
        RaycastHit hit;
        float rayLength = 25.0f;

        //Debug.DrawLine(transform.position, ahead, Color.red);
        //Debug.DrawRay(transform.position, ahead, Color.red);
        if(Physics.Raycast(transform.position, ahead, out hit, rayLength))
            {
                Debug.DrawLine(transform.position, ahead, Color.red);
                //Debug.DrawRay(transform.position, ahead, Color.red);
                Evade(hit.transform.gameObject);
            }
 
        }
        public void Evade(GameObject obstacle)
        {
            //find direction of target
            Vector3 direction = transform.position - obstacle.transform.position;
            direction.y = 0.0f;
    
            //calculate distance
            float distance = direction.magnitude;
    
            //if the target comes within proximity
            if(distance < 2.0f)
            {
                //rotate away from it
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), turnSpeed * Time.deltaTime);
                //move away from it
                moveVector = direction.normalized * maxVelocity* Time.deltaTime;
                transform.position += moveVector;
            }
        }

        public void Steer(Vector3 linearAcceleration, Vector3 targetPosition)
        {
            rb.velocity += linearAcceleration * Time.deltaTime;

            if (rb.velocity.magnitude > maxVelocity)
            {
                rb.velocity = rb.velocity.normalized * maxVelocity;
            }
            rb.AddRelativeForce(targetPosition, ForceMode.Force);
        }

        public static Vector3 OrientationToVector(float orientation, bool is3DGameObj)
        {
                /* Mulitply the orientation by -1 because counter clockwise on the y-axis is in the negative
                 * direction, but Cos And Sin expect clockwise orientation to be the positive direction */
            return new Vector3(Mathf.Cos(-orientation), 0, Mathf.Sin(-orientation));
        }

        public void moveAndAvoid( GameObject target){
            Vector3 targetPos = target.transform.position;
            targetPos.y = transform.position.y; //set y to zero so we only rotate on one plane
            Vector3 targetDir = targetPos - transform.position;
            rayArray[0] = transform.TransformDirection(-0.20f,0,0.5f); //ray pointed slightly left 
            rayArray[2] = transform.TransformDirection(0.20f,0,0.5f); //ray pointed slightly right 
            rayArray[1] = transform.forward; //ray 1 is pointed straight ahead
            //bool moveIt = false;
            bool midRay = false;
            bool leftRay = false;
            bool rightRay = false;
            //gameObject.SetActive(false);
            //loop through the rays
            //for (int i=0; i<3; i++) {
            RaycastHit hit;
            // if you hit something with the ray......
            if (Physics.Raycast (transform.position, rayArray[1], out hit, rayLength)) { 
                if (hit.collider.gameObject != target)
                {
                midRay = true;    
                Debug.DrawLine(transform.position, hit.point, Color.red);
                targetDir += mult * hit.normal;
                }
                
            } else {
                //moveIt = true;
                midRay = false;
            }

            if (Physics.Raycast (transform.position, rayArray[0], out hit, rayLength)) { 
                leftRay = true;    
                Debug.DrawLine(transform.position, hit.point, Color.red);
                targetDir += mult * hit.normal;

                
            } else {
                //moveIt = true;
                leftRay = false;
            }
            if (Physics.Raycast (transform.position, rayArray[2], out hit, rayLength)) { 
                rightRay = true;    
                Debug.DrawLine(transform.position, hit.point, Color.red);
                targetDir += mult * hit.normal;
             
            } else {
                //moveIt = true;
                rightRay = false;
            }
        //}
        // rotation and movement code 
            lerpedTargetDir = Vector3.Lerp(lerpedTargetDir,targetDir,Time.deltaTime * maxVelocity);
            Quaternion targetRotation = Quaternion.LookRotation(lerpedTargetDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed*Time.deltaTime);
            //gameObject.SetActive(false);
            if(!midRay)
            { 
                transform.Translate(Vector3.forward*Time.deltaTime*speed); 
                //gameObject.SetActive(false);
            }
            if(midRay){
                // if (!rightRay && leftRay){
                //     transform.Translate(Vector3.right*Time.deltaTime*speed); 
                // }

                if (!leftRay && rightRay)
                {
                    transform.Translate(-Vector3.right*Time.deltaTime*speed); 
                }
                else if (!rightRay && leftRay)
                {
                    transform.Translate(Vector3.right*Time.deltaTime*speed); 
                }
                else if (!rightRay && !leftRay) {
                    transform.Translate(Vector3.right*Time.deltaTime*speed); 
                }        
            }
        }
        private void OnCollisionEnter(Collision other) {
            if (other.gameObject == targetObject && other.gameObject.tag == "ShopEntrance" && goingShop){
                WaitThoseSeconds();
                //targetObject.gameObject.
                ShopSpawn ShopSpawnScript = other.gameObject.GetComponent<ShopSpawn>();
                if (!insideStoreAlready){
                transform.position = ShopSpawnScript.centerOfStore;
                insideStoreAlready = true;
                }
                else {
                transform.position = ShopSpawnScript.outOfStore;
                insideStoreAlready = false;
                goingShop = false;
                goingFood = true;
                }
                WaitThoseSeconds();
            }

            if (other.gameObject == targetObject && other.gameObject.tag == "MallExit" && !goingShop){
                //WaitThoseSeconds();
                //targetObject.gameObject.
                Destroy(gameObject); 
            }

            // if (other.gameObject == targetObject && other.gameObject.tag == "ShopEntrance" && other.transform.root.gameObject == ShopsDown){
            //     WaitThoseSeconds(3);
            //     //targetObject.gameObject.
            //     ShopSpawnDown ShopSpawnDownScript = targetObject.GetComponent<ShopSpawnDown>();
            //     transform.position = ShopSpawnDownScript.center;
            // }
        }

        IEnumerator WaitThoseSeconds()
        {
            float temp = speed;
            //Print the time of when the function is first called.
            Debug.Log("Started Coroutine at timestamp : " + Time.time);
            speed = 0;
            //yield on a new YieldInstruction that waits for 5 seconds.
            yield return new WaitForSeconds(3);
            //speed = temp;
            //After we have waited 5 seconds print the time again.
            Debug.Log("Finished Coroutine at timestamp : " + Time.time);
        }

        
    }   
}