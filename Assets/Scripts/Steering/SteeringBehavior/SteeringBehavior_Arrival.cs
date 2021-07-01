using UnityEngine;

namespace Steering
{
    public class SteeringBehavior_Arrival : SteeringBehavior
    {
        private float m_SlowingDistance = 10;
        private Vector3 m_DesiredVelocity = Vector3.zero;

        public override void PerformSteeringBehavior()
        {
            if (SteeringCore == null)
            {
                return;
            }

            if (SteeringCore.Target == null)
            {
                return;
            }

            // Calculate stopping factor
            float TargetDistance = (SteeringCore.Target - transform.position).magnitude;
            float stoppingFactor;

            if (m_SlowingDistance > 0)
            {
                stoppingFactor = Mathf.Clamp(TargetDistance / m_SlowingDistance, 0.0f, 1.0f);
            }
            else
            {
                stoppingFactor = Mathf.Clamp(TargetDistance, 0.0f, 1.0f);
            }
            m_DesiredVelocity = (SteeringCore.Target - transform.position).normalized * SteeringCore.MaxSpeed * stoppingFactor;
            SteeringForce = m_DesiredVelocity - SteeringCore.Velocity;
        }
        void OnDrawGizmos()
        {
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