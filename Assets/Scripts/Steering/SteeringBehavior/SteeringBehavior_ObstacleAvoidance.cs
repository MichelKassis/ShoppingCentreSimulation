using UnityEngine;

namespace Steering
{
    public class SteeringBehavior_ObstacleAvoidance : SteeringBehavior
    {
        [SerializeField]
        private LayerMask m_LayerMask;

        [SerializeField]
        private float m_BoundingSphereRadius = 1;
        [SerializeField]
        private float m_ObstacleMaxDistance = 10;

        [SerializeField]
        [Range(0.1f, 0.9f)]
        private float m_SteeringForceConservationTimer = 0;
        private Vector3 m_OldValidSteeringForce = Vector3.zero;
        private Vector3 m_DesiredVelocity = Vector3.zero;
        public override void PerformSteeringBehavior()
        {
            if (SteeringCore == null)
            {
                return;
            }
            Vector3 rayOrigin = transform.position;
            rayOrigin.y = transform.position.y/2;

            Ray ray = new Ray(rayOrigin, transform.forward);
            RaycastHit hitInfo;
            Vector3 avoidanceForce = Vector3.zero;

            if (Physics.SphereCast(ray, m_BoundingSphereRadius, out hitInfo, m_ObstacleMaxDistance, m_LayerMask))
            {
                if ( hitInfo.collider.gameObject != SteeringCore.targetObject)
                {
                    avoidanceForce = Vector3.Reflect(SteeringCore.Velocity, hitInfo.normal);

                    if (Vector3.Dot(avoidanceForce, SteeringCore.Velocity) < -0.9f)
                    {
                        avoidanceForce = transform.right;
                    }
                }
            }
            if (avoidanceForce != Vector3.zero)
            {
                m_DesiredVelocity = (avoidanceForce).normalized * SteeringCore.MaxSpeed;

                SteeringForce = m_DesiredVelocity - SteeringCore.Velocity;
                m_OldValidSteeringForce = SteeringForce;
                m_SteeringForceConservationTimer = 0;
            }
            else
            {
                SteeringForce = Vector3.zero;
            }
        }
        void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, m_BoundingSphereRadius);
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * m_ObstacleMaxDistance);

            if (SteeringCore == null)
            {
                return;
            }

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + m_DesiredVelocity);

            //if (SteeringCore.Rigidbody != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, transform.position + SteeringCore.Velocity);
            }
        }
    }
}