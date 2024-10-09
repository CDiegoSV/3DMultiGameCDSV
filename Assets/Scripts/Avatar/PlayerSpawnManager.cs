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

    [SerializeField] Transform[] spawnPositions;

    Transform spawnTransform;

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
        switch (LevelNetworkManager.Instance?.getCurrentPlayerCount)
        {
            case 1:
                spawnTransform = spawnPositions[0];
                break;
            case 2:
                spawnTransform = spawnPositions[1];
                break;
        }
        if(LevelNetworkManager.Instance == null)
        {
            spawnTransform = spawnPositions[0];
        }
        GameObject playerInstance = PhotonNetwork.Instantiate("Avatar", spawnTransform.position, Quaternion.identity);
        playerInstance.GetComponentInChildren<TextMeshProUGUI>().text = PhotonNetwork.NickName;
    }

    #endregion
}
