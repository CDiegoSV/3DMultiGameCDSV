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
    }

    private void Start()
    {
        if (LevelNetworkManager.Instance?.getCurrentPlayerCount > 0)
        {
            spawnTransform = spawnPositions[(int)LevelNetworkManager.Instance?.getCurrentPlayerCount - 1];
        }
        if(LevelNetworkManager.Instance == null)
        {
            spawnTransform = spawnPositions[0];
        }
        GameObject playerInstance = PhotonNetwork.Instantiate("Avatar", spawnTransform.position, Quaternion.identity);
        playerInstance.GetComponentInChildren<TextMeshProUGUI>().text = PhotonNetwork.NickName;
        playerInstance.transform.parent = playersParentGameObject.transform;
    }

    #endregion
}
