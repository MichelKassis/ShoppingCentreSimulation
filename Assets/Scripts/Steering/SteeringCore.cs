using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Steering
{
    [RequireComponent(typeof(Rigidbody))]
    public class SteeringCore : MonoBehaviour
    {
        private List<SteeringBehavior> allSteeringBehaviourList = new List<SteeringBehavior>();

        private Rigidbody rigidBodyComponent = null;

        [SerializeField]
        private float maxSteeringForce = 1;

        [SerializeField]
        private float maxSpeed = 10;

        public GameObject targetObject = null;
        public Vector3 targetObjectPosition = Vector3.zero;

        private Transform targetObjectTransform = null;
        private float m_RotationSyncScale = 6;
        private bool goingShop;
        private bool goingFood;
        private bool goingHome;
        private bool initalDecision = false;

        private GameObject[] shopEntrances;
        private GameObject[] chairs;

        private GameObject ShopsUp;
        private GameObject ShopsDown;

        private bool insideStoreAlready;
        private bool wanderMode = true;
        private bool trapMode = false;

        private bool pursuitMode = false;
        private IEnumerator waitCoroutine;

        
        public float MaxSteeringForce
        {
            get { 
                return maxSteeringForce;
                }
        }
        public Vector3 Velocity
        {
            get {
                return rigidBodyComponent.velocity;
                }
        }

        public Transform TargetTransform
        {
            get { return targetObjectTransform; }
            set { targetObjectTransform = value; }
        }

        public Vector3 Target
        {
            get { return targetObjectPosition; }
            set { targetObjectPosition = value; }
        }

        public float MaxSpeed
        {
            get { return maxSpeed; }
        }
        void Start()
        {
            
            initalDecision = false;
            shopEntrances = GameObject.FindGameObjectsWithTag("ShopEntrance");
            chairs = GameObject.FindGameObjectsWithTag("Chair");

            ShopsUp = GameObject.Find("/ShopsUp");
            ShopsDown = GameObject.Find("/ShopsDown");


            rigidBodyComponent = GetComponent<Rigidbody>();

            GetAllBehavioursForThisCharacter();

        }
        // Update is called once per frame
        void Update()
        {  
            if (gameObject.tag == "Shopper"){
            shopperUpdate();
            }
            
    
            else if (gameObject.tag == "AdMan"){
                AdManUpdate();
            }
            UpdateSteering();
            ApplySteering();
        }
        void OnDrawGizmos()
        {
            Vector3 steeringForceAverage = Vector3.zero;
            float priorityScale = 1;

            for (int i = 0; i < allSteeringBehaviourList.Count; i++)
            {
                if (allSteeringBehaviourList.Count > 1)
                {
                    priorityScale = allSteeringBehaviourList[i].BlendScale;
                }

                steeringForceAverage += allSteeringBehaviourList[i].SteeringForce * priorityScale;
            }

            steeringForceAverage.y = 0;

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + steeringForceAverage);
        }

        private void ApplySteering()
        {
            // Get steering force average
            Vector3 steeringForceAverage = Vector3.zero;
            float priorityScale = 1;
            for (int i = 0; i < allSteeringBehaviourList.Count; i++)
            {
                if (allSteeringBehaviourList.Count > 1)
                {
                    priorityScale = allSteeringBehaviourList[i].BlendScale;
                }

                steeringForceAverage += allSteeringBehaviourList[i].SteeringForce * priorityScale;
            }
            steeringForceAverage.y = 0;
            steeringForceAverage = Vector3.ClampMagnitude(steeringForceAverage, maxSteeringForce);

            // Add steering force to velocity
            rigidBodyComponent.velocity += steeringForceAverage;

            // Update rotation
            if (rigidBodyComponent.velocity.sqrMagnitude > 1)
            {
                
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(rigidBodyComponent.velocity), Time.deltaTime * m_RotationSyncScale);
            }
        }
        private void GetAllBehavioursForThisCharacter()
        {
            SteeringBehavior[] steeringBehaviors = GetComponents<SteeringBehavior>();
            for (int i = 0; i < steeringBehaviors.Length; i++)
            {
                allSteeringBehaviourList.Add(steeringBehaviors[i]);
            }
        }
        private void UpdateSteering()
        {
            for (int i = 0; i < allSteeringBehaviourList.Count; i++)
            {
                if (allSteeringBehaviourList[i].enabled)
                {
                    allSteeringBehaviourList[i].PerformSteeringBehavior();
                }
            }
        }
        private void shopperUpdate()
        {
            changeTargetForShopper();
        }

        private void AdManUpdate()
        {
            changeTargetForAdMan();
        }
        private void changeTargetForAdMan()
        {
                if (Random.value > 0.5f)
                {
                   wanderMode = true;
                   targetObject = null;
                }
                else 
                {
                //Pursuit mode
                wanderMode = false;
                pursuitMode = true;
                targetObject = GameObject.FindGameObjectWithTag("Shopper");
                targetObjectPosition = targetObject.transform.position;
                }
        }
        private void changeTargetForShopper()
        {
            //waitSecondsNormal(2);

            if (!initalDecision){

                if (Random.value > 0.5f)
                {
                    goingShop = false;
                    targetObject = GameObject.Find("MallExit");
                    targetObjectPosition = targetObject.transform.position;
                    initalDecision = true;
                }
                else 
                {
                    goingShop = true;
                    targetObject = shopEntrances[Random.Range(0,shopEntrances.Length-1)];
                    targetObjectPosition = targetObject.transform.position;
                    initalDecision = true;
                }
            }
            else {
                if (goingFood) {
                    targetObject = chairs[Random.Range(0,chairs.Length-1)];
                    targetObjectPosition = targetObject.transform.position;
                }
                else if (goingHome) {
                    targetObject = GameObject.Find("MallExit");
                    targetObjectPosition = targetObject.transform.position;
                }
            }
        }
        private void OnCollisionEnter(Collision other) {
            if (gameObject.tag == "Shopper"){


            if (other.gameObject == targetObject && other.gameObject.tag == "ShopEntrance" && goingShop){
                //gameObject.SetActive(false);
                //WaitThoseSeconds();
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
                goingHome = false;
                }
            }
            if (other.gameObject == targetObject && other.gameObject.tag == "Chair" && goingFood){
                
                goingShop = false;
                goingFood = false;
                goingHome = true;

            }

            if (other.gameObject == targetObject && other.gameObject.tag == "MallExit"){
                Destroy(gameObject); 
            }
            }

            else if (gameObject.tag == "AdMan"){
                if (other.gameObject == targetObject && other.gameObject.tag == "Shopper")
                {
                Material  material = other.gameObject.GetComponent<Renderer>().material;
                material.color = Color.red;   
            }

            }
        }

        IEnumerator WaitThoseSeconds(float seconds)
        {
            Debug.Log("Started Coroutine at timestamp : " + Time.time);
            yield return new WaitForSeconds(seconds);
            Debug.Log("Finished Coroutine at timestamp : " + Time.time);
        }
        public void waitSecondsNormal (float seconds){
            rigidBodyComponent.constraints = RigidbodyConstraints.FreezePosition;
            waitCoroutine = WaitThoseSeconds(seconds);
            StartCoroutine(waitCoroutine);
            rigidBodyComponent.constraints = RigidbodyConstraints.None;

        }

    }
}