using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObject : MonoBehaviour
{
    PhotonView m_PV;

    [SerializeField] List<GameObject> m_listGameObject;
    [SerializeField] List<MonoBehaviour> m_listScripts;

    void Start()
    {
        m_PV = GetComponent<PhotonView>();
        if(!m_PV.IsMine)
        {
            disableObjects();
            disableScripts();
        }
    }

    void disableObjects()
    {
        foreach (GameObject obj in m_listGameObject)
        {
            Destroy(obj);
        }
    }

    void disableScripts()
    {
        foreach (MonoBehaviour scripts in m_listScripts)
        {
            Destroy(scripts);
        }
    }
}
