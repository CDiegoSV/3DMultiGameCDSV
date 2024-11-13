using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviourPunCallbacks
{
    public static UIManager Instance;

    [SerializeField] private TextMeshProUGUI winPanelText;
    [SerializeField] private GameObject winPanel;

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

    public void ActivateWinPanel()
    {
        winPanel.SetActive(true);
    }

    public void ChangeWinPanelTextNColor(string p_text, Color p_color)
    {
        winPanelText.text = p_text;
        winPanelText.color = p_color;
    }
}
