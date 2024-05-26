using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public GameObject EnterNamePanel;
    public GameObject ConnectionStatusPanel;
    public GameObject LobbyPanel;

    public static CanvasManager instance;

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

    public void JoinRandomRoom()
    {
        LaunchManager.instance.joinRandomRoom();
    }

}
