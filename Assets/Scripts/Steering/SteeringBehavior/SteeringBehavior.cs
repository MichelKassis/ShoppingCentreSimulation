using UnityEngine;

namespace Steering
{
    [RequireComponent(typeof(SteeringCore))]
    public abstract class SteeringBehavior : MonoBehaviour
    {
        private uint blendScale = 1;

        private SteeringCore steeringCore = null;

        
        private Vector3 steeringForce = Vector3.zero;

        public float BlendScale
        {
            get { return blendScale; }
        }

        public Vector3 SteeringForce
        {
            get { return steeringForce; }
            set { steeringForce = value; }
        }

        protected SteeringCore SteeringCore
        {
            get { return steeringCore; }
        }
        void Start()
        {
            steeringCore = GetComponent<SteeringCore>();
        }
        public abstract void PerformSteeringBehavior();

    }
}