using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Linq;


/// <summary>
/// Manages all level events, roles, win or lose conditions.
/// </summary>
public class LevelManager : MonoBehaviourPunCallbacks
{
    public static LevelManager instance;

    PhotonView m_PV;

    LevelManagerState m_currentState;

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

    public void SetLevelManagerState(LevelManagerState p_newState)
    {
        if(p_newState == m_currentState)
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
            case LevelManagerState.Playing:
                Playing();
                break;
        }
    }

    /// <summary>
    /// Inicializa el estado de Playing.
    /// </summary>
    void Playing()
    {
        AssignRole();
        setNewRoleEvent();
    }

    private void Start()
    {
        m_PV = GetComponent<PhotonView>();
        SetLevelManagerState(LevelManagerState.Waiting);

        if (PhotonNetwork.IsMasterClient)
        {

        }
    }

    public LevelManagerState GetCurrentState
    {
        get { return m_currentState; }
    }


    /// <summary>
    /// Levanta el evento cuando los jugadores esten listos para la partida.
    /// </summary>
    private void setNewRoleEvent()
    {
        byte MoveUnitsToTargetPositionEventCode = 1;
        object content = "Starting Game";
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All};
        PhotonNetwork.RaiseEvent(MoveUnitsToTargetPositionEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public LevelManagerState GetLevelManagerState()
    {
        return m_currentState;
    }

    //Falta asignar cuántos roles hay según el jugador
    void AssignRole()
    {
        print("Asignación de rol");
        Player[] m_playersArray = PhotonNetwork.PlayerList;
        GameplayRole[] m_gameplayRole = { GameplayRole.Innocent, GameplayRole.Traitor };

        m_gameplayRole = m_gameplayRole.OrderBy(x => Random.value).ToArray();

        for(int i = 0; i > m_playersArray.Length; ++i)
        {
            Hashtable m_playerProperties = new Hashtable();
            m_playerProperties["Role"] = m_gameplayRole[i % m_gameplayRole.Length].ToString();
            m_playersArray[i].SetCustomProperties(m_playerProperties);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 4)
        {
            StartCoroutine(CreateAndSendEventTimerCorutine());
        }
    }

    IEnumerator CreateAndSendEventTimerCorutine()
    {
        yield return new WaitForSeconds(3f);
        SetLevelManagerState(LevelManagerState.Playing);
    }

}

public enum LevelManagerState
{
    None,
    Waiting,
    Playing
}

public enum GameplayRole
{
    Innocent,
    Traitor,
}
