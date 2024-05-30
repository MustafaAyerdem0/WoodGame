using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using PlayFab;
using TMPro;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public GameObject AuthPanel;
    public GameObject ConnectionStatusPanel;
    public static CanvasManager instance;

    [SerializeField] public TMP_Text L_mailText;
    [SerializeField] public TMP_Text L_passwordText;

    [SerializeField] public TMP_Text R_mailText;
    [SerializeField] public TMP_Text R_playerNameText;
    [SerializeField] public TMP_Text R_passwordText;

    public TMP_Text statusText;
    public GameObject MatchmakingPanel;
    public GameObject playerNamePanel;
    public GameObject WoodPanel;
    public TMP_Text playerDisplayName;
    public TMP_Text playerWoodCount;

    public TMP_Text player1Name;
    public TMP_Text player2Name;

    public GameObject startMatchmakingButton, cancelMatcmakingButton;
    public TMP_Text matchmakingTimer;



    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        else instance = this;
    }

    private void OnEnable()
    {
        PlayfabManager.onDataReceivedAction += CloseAuthPanel;
        PlayfabManager.onDataReceivedAction += UpdateCollectedWoodText;
    }

    private void OnDisable()
    {
        PlayfabManager.onDataReceivedAction -= CloseAuthPanel;
        PlayfabManager.onDataReceivedAction -= UpdateCollectedWoodText;
    }

    void Start()
    {
        if (!PlayFabClientAPI.IsClientLoggedIn()) AuthPanel.SetActive(true);
        else OpenProfilePanel();

        ConnectionStatusPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
    }

    public void CreateMatchmakingTicket()
    {
        LaunchManager.instance.CreateMatchmakingTicket();
        matchmakingTimer.gameObject.SetActive(true);
        cancelMatcmakingButton.SetActive(true);
        startMatchmakingButton.SetActive(false);
    }

    public void CancelMatchmakingTicket()
    {
        LaunchManager.instance.CancelMatchmakingTicket();
        matchmakingTimer.gameObject.SetActive(false);
        cancelMatcmakingButton.SetActive(false);
        startMatchmakingButton.SetActive(true);
    }

    public void CloseAuthPanel()
    {
        ConnectionStatusPanel.SetActive(false);
        AuthPanel.SetActive(false);
    }

    public void UpdateCollectedWoodText()
    {
        playerWoodCount.text = PlayfabManager.instance.playerProperty.collectedWoodCount.ToString();
    }

    public void OpenProfilePanel()
    {
        playerDisplayName.text = PlayfabManager.displayName;
        playerNamePanel.SetActive(true);
        WoodPanel.SetActive(true);
    }

}
