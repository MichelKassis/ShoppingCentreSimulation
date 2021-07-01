using UnityEngine;

namespace Steering
{
    public class SteeringBehavior_Pursuit : SteeringBehavior
    {
        private Vector3 m_TargetOldPosition = Vector3.zero;

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
            // Predict target future position depending on distance
            Vector3 targetVelocity = SteeringCore.Target - m_TargetOldPosition;
            float targetDistance = (SteeringCore.Target - transform.position).magnitude;
            float targetVelocityMagnitude = targetVelocity.magnitude;
            float targetVelocityScale = targetDistance / SteeringCore.MaxSpeed;

            // Calculate desired velocity
            m_DesiredVelocity = (SteeringCore.Target + (targetVelocity * targetVelocityScale) - transform.position).normalized * SteeringCore.MaxSpeed;

            // Calculate steering force
            SteeringForce = m_DesiredVelocity - SteeringCore.Velocity;

            // Keep old target position
            m_TargetOldPosition = SteeringCore.Target;
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