using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;




public class AvatarBehaviour : MonoBehaviourPunCallbacks, IOnEventCallback
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
    [SerializeField] protected ParticleSystem _particleSystem;

    [SerializeField] private SpriteRenderer _roleIndicator;


    #endregion

    #region Runtime Variables

    protected Vector3 avatarDirection;
    protected Quaternion avatarRotation;

    [SerializeField] protected int m_life;

    protected bool m_roleReady;
    [SerializeField] protected bool m_imTraitor;
    #endregion

    #region Unity Methods


    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void Start()
    {
        InitializeAvatar();
    }

    private void Update()
    {
        if (_photonView.IsMine)
        {
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
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
            //OnLifein0();
            print(gameObject.name + " desvivido.");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_photonView.IsMine && (other.CompareTag("Agent") || other.CompareTag("Player") ) && Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log("Kill Agent Input Detection");
            _animator.SetTrigger("Attack");
            other.gameObject.GetComponent<AvatarBehaviour>().GettingDamage();
        }
        
    }

    #endregion

    #region Public Methods

    public virtual void GettingDamage()
    {
        m_life--;
    }

    private void GetNewGameplayRole()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Role", out object role) && m_roleReady == false)
        {
            string m_newPlayerRole = role.ToString();
            print("El rol es: " + m_newPlayerRole);

            if(_roleIndicator != null)
            {
                switch (m_newPlayerRole)
                {
                    case "Innocent":
                        //Soy inocente
                        _roleIndicator.color = Color.blue;
                        _photonView.RPC("SetTraitorBool", RpcTarget.AllBuffered, false);
                        Debug.Log("Soy Traidor?: " + m_imTraitor);
                        break;
                    case "Traitor":
                        //Soy una sucia rata
                        _roleIndicator.color = Color.red;
                        _photonView.RPC("SetTraitorBool", RpcTarget.AllBuffered, true);
                        Debug.Log("Soy Traidor?: " + m_imTraitor);
                        break;
                }
            }
            m_roleReady = true;
        }
    }
    #endregion

    #region Local Methods

    private void InitializeAvatar()
    {
        if (!_photonView.IsMine)
        {
            //gameObject.GetComponentInChildren<TextMeshProUGUI>().text = _photonView.Owner.NickName;
        }
        if(_photonView.IsMine) 
        {
            m_life = 1;
            _cam = GameObject.FindFirstObjectByType<Cinemachine.CinemachineFreeLook>();
            _cam.Follow = transform;
            _cam.LookAt = transform;
            m_roleReady = false;
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

    [PunRPC]
    private void SetTraitorBool(bool value)
    {
        m_imTraitor = value;
    }

    protected virtual IEnumerator DeathCorutine()
    {
        _particleSystem.Play();
        yield return new WaitForSeconds(_deathClip.length);
        if(m_imTraitor == true)
        {
            TraitorDiedEvent();
        }
        else
        {
            InnocentDiedEvent();
        }
        Destroy(gameObject);
    }


    


    void TraitorDiedEvent()
    {
        byte m_ID = 2;
        object content = "Un traidor ha muerto.";
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };

        PhotonNetwork.RaiseEvent(m_ID, content, raiseEventOptions, SendOptions.SendReliable);
    }
    
    void InnocentDiedEvent()
    {
        byte m_ID = 3;
        object content = "Un inocente ha muerto.";
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };

        PhotonNetwork.RaiseEvent(m_ID, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        switch (eventCode)
        {
            case 1:
                GetNewGameplayRole();
                break;
        }

        if(eventCode == 1)
        {
            
        }
    }





    #endregion
}
