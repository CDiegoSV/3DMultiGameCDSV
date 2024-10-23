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
    [SerializeField, HideInInspector] protected Rigidbody _rigidBody;
    [SerializeField, HideInInspector] protected PhotonView _photonView;
    [SerializeField, HideInInspector] protected Animator _animator;
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
        if(m_life == 0)
        {
            print("A");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Agent" && Input.GetKeyDown(KeyCode.Mouse0))
        {
            DamageOtherPlayer(other.GetComponent<PhotonView>().Owner);
        }
    }

    #endregion

    #region Public Methods

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if(changedProps.ContainsKey("damage"))
        {
            m_life -= (int)changedProps["damage"];
        }
    }

    #endregion

    #region Local Methods

    private void InitializeAvatar()
    {
        if (!_photonView.IsMine)
        {
            gameObject.GetComponentInChildren<TextMeshProUGUI>().text = _photonView.Owner.NickName;
        }
        else
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


    private void DamageOtherPlayer(Player p_otherPlayer)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            Hashtable playerStats = new Hashtable();

            playerStats["damage"] = 1;
            p_otherPlayer.SetCustomProperties(playerStats);
        }
    }


    #endregion
}
