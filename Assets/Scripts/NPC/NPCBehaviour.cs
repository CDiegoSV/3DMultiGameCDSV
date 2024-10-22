using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCBehaviour : MonoBehaviour
{
    #region References

    [SerializeField] NavMeshAgent m_agent;
    [SerializeField] float m_moveRadius;

    #endregion

    #region Runtime Variables

    protected Vector3 m_randomDirection;

    #endregion

    #region Unity Methods

    private void FixedUpdate()
    {
        if(!m_agent.pathPending && m_agent.remainingDistance < 0.3f)
        {
            MoveToRandomPosition();
        }
        
    }

    #endregion

    #region Local Methods

    protected void MoveToRandomPosition()
    {
        m_randomDirection = Random.insideUnitSphere * m_moveRadius;

        m_randomDirection += transform.position;

        NavMeshHit m_hit;
        if(NavMesh.SamplePosition(m_randomDirection, out m_hit, m_moveRadius, NavMesh.AllAreas))
        {
            m_agent.SetDestination(m_hit.position);
        }
    }

    #endregion
}
