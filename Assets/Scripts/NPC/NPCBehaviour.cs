using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCBehaviour : AvatarBehaviour
{
    #region References

    [SerializeField] NavMeshAgent m_agent;
    [SerializeField] float m_moveRadius;

    #endregion

    #region Runtime Variables

    protected Vector3 m_randomDirection;


    #endregion



    #region Unity Methods

    private void Start()
    {
        _photonView = GetComponent<PhotonView>();
        _animator = GetComponentInChildren<Animator>();
        _animator.SetInteger("Velocity", 1);
        m_life += 1;
    }

    private void FixedUpdate()
    {
        if (m_life == 0)
        {
            Debug.Log("Muertoo");
            _photonView.RPC("OnLifein0", RpcTarget.AllBuffered);
            //OnLifein0();
        }
        //if (!m_agent.pathPending && m_agent.remainingDistance < 0.3f)
        //{
        //    MoveToRandomPosition();

        //}

    }


    #endregion

    public override void GettingDamage()
    {
        if (m_life >= 0)
        {
            m_life--;
        }
        else
        {
            Debug.Log("Ya se murió");
        }
    }

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

    [PunRPC]
    protected override void OnLifein0()
    {
        m_life--;
        _animator.SetTrigger("Death");
        StartCoroutine(DeathCorutine());
    }


    protected override IEnumerator DeathCorutine()
    {
        _particleSystem.Play();
        Debug.Log("Inside Coroutine, animator called");
        yield return new WaitForSeconds(_deathClip.length);
        Destroy(gameObject);
    }
    #endregion
}
