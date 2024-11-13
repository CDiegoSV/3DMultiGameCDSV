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
        assignRole();
        setNewRoleEvent();
    }

    //Falta asignar cuantos roles hay segun la cantidad de jugadores
    void assignRole()
    {
        print("Se crea Hastable con la asignacion del nuevo rol");
        Player[] m_playersArray = PhotonNetwork.PlayerList;
        List<GameplayRole> m_gameplayRoleList = new List<GameplayRole>();

        //m_gameplayRole = m_gameplayRole.OrderBy(x => Random.value).ToArray();

        if(m_playersArray.Length <= 4)
        {
            m_gameplayRoleList.Add(GameplayRole.Traitor);
            m_traitorsAlive++;
            for(int i = m_gameplayRoleList.Count;  i < m_playersArray.Length; i++)
            {
                m_gameplayRoleList.Add(GameplayRole.Innocent);
                m_innocentsAlive++;
            }
        }

        for (int i = 0; i < m_playersArray.Length; i++)
        {

            int index = Random.Range(0, m_gameplayRoleList.Count);
            Hashtable m_playerProperties = new Hashtable();

            //m_playerProperties["Role"] = m_gameplayRole[i % m_gameplayRole.Length].ToString();
            m_playerProperties["Role"] = m_gameplayRoleList[index].ToString();
            m_gameplayRoleList.RemoveAt(index);
            m_playersArray[i].SetCustomProperties(m_playerProperties);
        }
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