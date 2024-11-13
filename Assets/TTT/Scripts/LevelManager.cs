using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Linq;

public class LevelManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static LevelManager instance;

    [SerializeField] int m_traitorPercent;


    [SerializeField] int m_innocentsAlive;
    [SerializeField] int m_traitorsAlive;

    PhotonView m_photonView;
    LevelManagerState m_currentState;

    bool m_canReceiveEvents;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }

    void Start()
    {
        m_photonView = GetComponent<PhotonView>();

        setLevelManagerState(LevelManagerState.Waiting);

        if(PhotonNetwork.IsMasterClient)
        {
            print("Masteeeeeer");
        }
        m_canReceiveEvents = true;
    }
    /// <summary>
    /// Levanta el Evento cuando los jugadores esten listos para la partida
    /// </summary>
    void setNewRoleEvent()
    {
        byte m_ID = 1;//Codigo del Evento (1...199)
        object content = "Asignacion de nuevo rol...";
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

        PhotonNetwork.RaiseEvent(m_ID, content, raiseEventOptions, SendOptions.SendReliable);
    }
    public LevelManagerState CurrentState { get { return m_currentState; } }
    public LevelManagerState getLevelManagerSate()
    {
        return m_currentState;
    }

    public void setLevelManagerState(LevelManagerState p_newState)
    {
        if (p_newState == m_currentState)
        {
            return;
        }
        m_currentState = p_newState;

        switch (m_currentState)
        {
            case LevelManagerState.None:
                break;
            case LevelManagerState.Waiting:
                break;

            case LevelManagerState.Starting:
                starting();
                break;

            case LevelManagerState.Playing:
                break;

            case LevelManagerState.Finishing:
                break;
        }

    }
    /// <summary>
    /// Inicializa el estado de Playing
    /// </summary>
    void starting()
    {
        assignRoles();
        setNewRoleEvent();
    }


    //Falta asignar cuantos roles hay segun la cantidad de jugadores
    void assignRoles()
    {
        print("Se crea Hastable con la asignacion del nuevo rol");
        Player[] m_playersArray = PhotonNetwork.PlayerList;
        List<GameplayRole> m_gameplayRoleList = new List<GameplayRole>();
        int totalPlayers = m_playersArray.Length;

        int traitorCount = Mathf.Max(1, Mathf.RoundToInt(totalPlayers * m_traitorPercent));
        int innocentCount = totalPlayers - traitorCount;

        m_traitorsAlive = traitorCount;
        m_innocentsAlive = innocentCount;

        m_gameplayRoleList.AddRange(Enumerable.Repeat(GameplayRole.Traitor, traitorCount));
        m_gameplayRoleList.AddRange(Enumerable.Repeat(GameplayRole.Innocent, innocentCount));

        ShuffleRoleList(m_gameplayRoleList);

        for (int i = 0; i < m_playersArray.Length; i++)
        {
            Hashtable m_playerProperties = new Hashtable();

            m_playerProperties["Role"] = m_gameplayRoleList[i].ToString();
            m_playersArray[i].SetCustomProperties(m_playerProperties);
        }
    }

    void ShuffleRoleList(List<GameplayRole> p_rolesList)
    {
        for (int i = p_rolesList.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            GameplayRole temp_role = p_rolesList[i];
            p_rolesList[i] = p_rolesList[j];
            p_rolesList[j] = temp_role;
        }
    }

    IEnumerator FinishingGameCoroutine(string p_text, Color p_textColor)
    {
        UIManager.Instance.ChangeWinPanelTextNColor(p_text, p_textColor);
        UIManager.Instance.ActivateWinPanel();

        yield return new WaitForSeconds(4f);

        //Go to MainMenu
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 4)
        {
            StartCoroutine(timerToStart());
        }
    }

    //Probablemente Se necesite RPC
    IEnumerator timerToStart()
    {
        yield return new WaitForSeconds(3);
        setLevelManagerState(LevelManagerState.Starting);
    }

    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case 2: //A Traitor Died
                if(m_traitorsAlive > 0 && m_canReceiveEvents == true)
                {
                    m_traitorsAlive--;
                    Debug.Log(gameObject.name + "A Traitor Died " + m_photonView.Owner.NickName);

                    if (m_traitorsAlive == 0)
                    {
                        print("Inocentes Ganan");
                        m_photonView.RPC("StartFisinishingCorutine", RpcTarget.All, false);
                        setLevelManagerState(LevelManagerState.Finishing);
                    }
                    StartCoroutine(CanReceiveEventsCooldown());
                }

                break;

            case 3: //A Innocent Died
                if(m_innocentsAlive > 0 && m_canReceiveEvents == true)
                {
                    m_innocentsAlive--;
                    Debug.Log(gameObject.name + " A Innocent Died " + m_photonView.Owner.NickName);

                    if (m_innocentsAlive == 0)
                    {
                        print("Traidores Ganan");
                        m_photonView.RPC("StartFisinishingCorutine", RpcTarget.All, true);
                        setLevelManagerState(LevelManagerState.Finishing);
                    }
                    StartCoroutine(CanReceiveEventsCooldown());
                }
                break;
        }
    }
    
    IEnumerator CanReceiveEventsCooldown()
    {
        m_canReceiveEvents = false;
        yield return new WaitForSeconds(2f);
        m_canReceiveEvents = true;
    }

    [PunRPC]
    private void StartFisinishingCorutine(bool p_traitorWin)
    {
        if(p_traitorWin == false)
        {
            StartCoroutine(FinishingGameCoroutine("Innocents Win", Color.blue));
        }
        else
        {
            StartCoroutine(FinishingGameCoroutine("Traitors Win", Color.red));
        }
    }

    //private void OnEnable() {
    //    PhotonNetwork.AddCallbackTarget(this);
    //}

    //private void OnDisable() {
    //    PhotonNetwork.RemoveCallbackTarget(this);
    //}

    //public void OnEvent(EventData photonEvent)
    //{
    //    byte eventCode = photonEvent.Code;
    //    if (eventCode == 1)
    //    {
    //        string data = (string)photonEvent.CustomData;
    //        //getNewGameplayRole();
    //        //Hacer algo con el string
    //    }
    //}
}
public enum LevelManagerState
{
    None,
    Waiting,
    Starting,
    Playing,
    Finishing
}


public enum GameplayRole
{
    Innocent,
    Traitor
}