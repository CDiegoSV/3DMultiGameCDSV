using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class AvatarBehaviour : MonoBehaviour
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

    #endregion

    #region Runtime Variables

    protected Vector3 avatarDirection;
    protected Quaternion avatarRotation;

    #endregion

    #region Unity Methods

    private void FixedUpdate()
    {
        if (_photonView.IsMine && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
        {
            AvatarRBMove();
        }
        else if (_photonView.IsMine)
        {
            _animator.SetInteger("Velocity", 0);
        }

    }

    #endregion

    #region Public Methods



    #endregion

    #region Private Methods

    private void AvatarRBMove()
    {
        avatarDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        if (avatarDirection.magnitude > 0 )
        {
            avatarRotation = Quaternion.LookRotation(avatarDirection);
        }
        _rigidBody.Move(_rigidBody.position +  m_speed * Time.fixedDeltaTime * avatarDirection, Quaternion.Euler(0, avatarRotation.eulerAngles.y, 0));
        _animator.SetInteger("Velocity", (int)avatarDirection.magnitude);

    }

    #endregion
}
