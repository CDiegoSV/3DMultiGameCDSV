using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PhotonConnection : MonoBehaviourPunCallbacks
{

    #region References
    [Header("Button References")]
    [SerializeField] TMP_InputField inputField;
    [SerializeField] GameObject okButton;
    [SerializeField] GameObject playerNameText;

    [Header("Loading Panel References")]
    [SerializeField] GameObject loadingPanel;
    [SerializeField] GameObject ghostyObject;
    [SerializeField] Animator panelAnimator;
    [SerializeField] AnimationClip outClip;

    #endregion

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        print("Se ha conectado al Master");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.NickName = "";
        print("Ha entrado al lobby Ab");
        panelAnimator.SetBool("PanelOut", true);
        Invoke("LoadingPanelsSet", outClip.length);
        //PhotonNetwork.JoinOrCreateRoom("TestRoom", newRoomInfo(), null);
    }

    public override void OnJoinedRoom()
    {
        print("Entro a Room: " + PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.LoadLevel("AvatarTest");
        //PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        print("Error al crear Room: " + message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        print("Error al intentar unirse al Room: " + message);
    }

    RoomOptions newRoomInfo()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 5;
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;

        return roomOptions;
    }

    public void LoadingPanelsSet()
    {
        loadingPanel.gameObject.SetActive(false);
        ghostyObject.SetActive(true);
    }

    public void OnClickOkButton()
    {
        if (inputField.text != "")
        {
            PhotonNetwork.NickName = inputField.text;
        }
        else
        {
            inputField.gameObject.SetActive(true);
            okButton.gameObject.SetActive(true);
            playerNameText.gameObject.SetActive(false);
        }
    }

    public void OnClickPlayButton()
    {
        if(PhotonNetwork.NickName != "")
        {
            PhotonNetwork.JoinOrCreateRoom("TestRoom", newRoomInfo(), null);
        }
    }

    public void OnClickQuitButton()
    {
        Application.Quit();
    }
}
