using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelNetworkManager : MonoBehaviourPunCallbacks
{
    public static LevelNetworkManager Instance { get; private set; }

    #region Unity Methods

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
    }

    #endregion

    #region Public Methods

    public void DisconnectCurrentRoom()
    {
        PhotonNetwork.LeaveRoom();
    }




    #endregion

    #region PUN Methods


    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("Menu");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        print("Entró nuevo usuario: " + newPlayer.NickName);

    }

    #endregion

    #region Getters And Setters

    public int getCurrentPlayerCount
    {
        get
        {
            return PhotonNetwork.CurrentRoom.PlayerCount;
        }
    }

    #endregion
}
