using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    public static PlayerSpawnManager instance;

    PhotonView myPV;

    Transform spawnTransform;
    [SerializeField] Transform playersParentGameObject;

    [SerializeField] Transform[] spawnPositions;


    #region Unity Methods

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        myPV = GetComponent<PhotonView>();

        if (LevelNetworkManager.Instance?.getCurrentPlayerCount > 0)
        {
            spawnTransform = spawnPositions[(int)LevelNetworkManager.Instance?.getCurrentPlayerCount - 1];
        }
        if(LevelNetworkManager.Instance == null)
        {
            spawnTransform = spawnPositions[0];
        }
        GameObject playerInstance = PhotonNetwork.Instantiate("Avatar", spawnTransform.position, Quaternion.identity);
        playerInstance.transform.parent = playersParentGameObject.transform;

        if(myPV.IsMine)
        {
            GameObject lvlManager = PhotonNetwork.Instantiate("LvlManager", spawnTransform.position, Quaternion.identity);
        }
    }

    #endregion
}
