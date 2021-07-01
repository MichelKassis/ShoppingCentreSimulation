using UnityEngine;

namespace Steering
{
    public class SteeringBehavior_Wander : SteeringBehavior
    {
        private float   m_WanderRadius          = 5;
        private float   m_WanderDistance        = 5; 
        private float   m_RandomPointFrequency  = 1;
        private float   m_Timer                 = 0;

        private Vector3 m_RandomPoint           = Vector3.zero;

        private Vector3 m_WanderCirclePosition  = Vector3.zero;
        private Vector3 m_DesiredVelocity       = Vector3.zero;

        public override void PerformSteeringBehavior()
        {
            if (SteeringCore == null)
            {
                return;
            }

            // Calculate desired velocity
            m_WanderCirclePosition = transform.position + transform.forward * m_WanderDistance;
           
            m_DesiredVelocity = (m_RandomPoint - transform.position).normalized * SteeringCore.MaxSpeed;
            
            SteeringForce = m_DesiredVelocity - SteeringCore.Velocity;
            UpdateTimer();
        }
        // Gizmos
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

            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, transform.position + SteeringCore.Velocity.normalized * m_WanderDistance);
            Gizmos.DrawWireSphere(transform.position + SteeringCore.Velocity.normalized * m_WanderDistance, m_WanderRadius);
            Gizmos.DrawWireSphere(m_RandomPoint, 0.33f);
            Gizmos.DrawLine(transform.position + SteeringCore.Velocity.normalized * m_WanderDistance, m_RandomPoint);
        }
        private void UpdateTimer()
        {
            m_Timer += Time.deltaTime;

            if (m_Timer > m_RandomPointFrequency)
            {
                m_RandomPoint = Random.insideUnitCircle * m_WanderRadius;
                m_RandomPoint.z = m_RandomPoint.y;
                m_RandomPoint.y = 0;
                m_RandomPoint += m_WanderCirclePosition;
                m_Timer = 0;
            }
        }

    }
}