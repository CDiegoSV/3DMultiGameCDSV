using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;




public class AvatarBehaviour : MonoBehaviourPunCallbacks
{
    #region Knobs
    [Header("Avatar Knobs")]

    [SerializeField] protected float m_speed;

    #endregion

    #region References
    [Header("Avatar Component References")]
    [SerializeField] protected Rigidbody _rigidBody;
    [SerializeField] protected PhotonView _photonView;
    [SerializeField] protected Animator _animator;
    [SerializeField] protected AnimationClip _deathClip;
    protected Cinemachine.CinemachineFreeLook _cam;

    #endregion

    #region Runtime Variables

    protected Vector3 avatarDirection;
    protected Quaternion avatarRotation;

    protected int m_life;

    #endregion

    #region Unity Methods


    private void Start()
    {
        InitializeAvatar();
    }

    private void Update()
    {
        if (_photonView.IsMine)
        {
            avatarDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
            _animator.SetInteger("Velocity", (int)avatarDirection.magnitude);
        }
    }

    private void FixedUpdate()
    {
        if (_photonView.IsMine && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
        {
            AvatarRBMove();
        }
        if (m_life == 0)
        {
            _photonView.RPC("OnLifein0", RpcTarget.AllBuffered);
            print(gameObject.name + " desvivido.");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (other.GetComponent<AvatarBehaviour>() != null)
            {
                DamageOtherPlayer(other.GetComponent<AvatarBehaviour>());
            }
            else
            {
                DamageOtherPlayer(other.GetComponent<NPCBehaviour>());
            }
        }
    }

    #endregion

    #region Public Methods

    public virtual void GettingDamage()
    {
        m_life--;
    }
   
    #endregion

    #region Local Methods

    private void InitializeAvatar()
    {
        if (!_photonView.IsMine)
        {
            gameObject.GetComponentInChildren<TextMeshProUGUI>().text = _photonView.Owner.NickName;
        }
        if(_photonView.IsMine) 
        {
            m_life = 1;
            _cam = GameObject.FindFirstObjectByType<Cinemachine.CinemachineFreeLook>();
            _cam.Follow = transform;
            _cam.LookAt = transform;
        }
    }

    private void AvatarRBMove()
    {
        if (avatarDirection.magnitude > 0 )
        {
            avatarRotation = Quaternion.LookRotation(avatarDirection);
        }
        _rigidBody.Move(_rigidBody.position +  m_speed * Time.fixedDeltaTime * avatarDirection, Quaternion.Euler(0, avatarRotation.eulerAngles.y, 0));
    }

    private void DamageOtherPlayer(AvatarBehaviour otherPlayer)
    {
        if (_photonView.IsMine)
        {
            otherPlayer.GettingDamage();
        }
    }

    [PunRPC]
    protected virtual void OnLifein0()
    {
        m_life--;
        _animator.SetTrigger("Death");
        StartCoroutine(DeathCorutine());
    }

    protected virtual IEnumerator DeathCorutine()
    {
        yield return new WaitForSeconds(_deathClip.length);
        PhotonNetwork.Destroy(gameObject);
    }

    #endregion
}
