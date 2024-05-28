using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public GameObject EnterNamePanel;
    public GameObject ConnectionStatusPanel;
    public GameObject LobbyPanel;

    public static CanvasManager instance;
    public TMP_Text statusText;
    public GameObject versusPanel;
    public TMP_Text player1Name;
    public TMP_Text player2Name;

    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        else instance = this;
    }

    void Start()
    {
        EnterNamePanel.SetActive(true);
        ConnectionStatusPanel.SetActive(false);
        //LobbyPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
    }

}
