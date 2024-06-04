using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using PlayFab;
using TMPro;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager instance;
    public static Action onLaunchScreenOpenedWithoutLoginAction;


    [Header("GameObjects")]

    [SerializeField] public GameObject LoadingBar;
    [SerializeField] public GameObject AuthPanel;
    [SerializeField] public GameObject MatchmakingPanel;
    [SerializeField] public GameObject playerNamePanel;
    [SerializeField] public GameObject WoodPanel;
    [SerializeField] public GameObject startMatchmakingButton, cancelMatcmakingButton, startSingleGameButton;

    [Header("Texts")]

    [SerializeField] public TMP_Text L_mailText;
    [SerializeField] public TMP_Text L_passwordText;
    [SerializeField] public TMP_Text R_mailText;
    [SerializeField] public TMP_Text R_playerNameText;
    [SerializeField] public TMP_Text R_passwordText;
    [SerializeField] public TMP_Text statusText;
    [SerializeField] public TMP_Text playerDisplayName;
    [SerializeField] public TMP_Text playerWoodCount;
    [SerializeField] public TMP_Text player1Name;
    [SerializeField] public TMP_Text player2Name;
    [SerializeField] public TMP_Text matchmakingTimer;
    [SerializeField] public TMP_Text launchCountText;

    [Header("Toggles")]
    [SerializeField] public Toggle womanToggle;
    [SerializeField] public Toggle manToggle;



    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        else instance = this;
    }

    private void OnEnable()
    {
        PlayfabManager.onDataReceivedAction += CloseAuthPanel;
        PlayfabManager.onDataReceivedAction += UpdateCollectedWoodText;
        PlayfabManager.onDataReceivedAction += UpdateLanguageFromDb;
        AzureFunction.onGetLaunchCountSuccess += UpdateLaunchCountText;
        onLaunchScreenOpenedWithoutLoginAction += UpdateLaunchCountText;
        onLaunchScreenOpenedWithoutLoginAction += OpenProfilePanel;
        onLaunchScreenOpenedWithoutLoginAction += UpdateCollectedWoodText;
        onLaunchScreenOpenedWithoutLoginAction += UpdateLanguageFromDb;
    }

    private void OnDisable()
    {
        PlayfabManager.onDataReceivedAction -= CloseAuthPanel;
        PlayfabManager.onDataReceivedAction -= UpdateCollectedWoodText;
        PlayfabManager.onDataReceivedAction -= UpdateLanguageFromDb;
        AzureFunction.onGetLaunchCountSuccess -= UpdateLaunchCountText;
        onLaunchScreenOpenedWithoutLoginAction -= UpdateLaunchCountText;
        onLaunchScreenOpenedWithoutLoginAction -= OpenProfilePanel;
        onLaunchScreenOpenedWithoutLoginAction -= UpdateCollectedWoodText;
        onLaunchScreenOpenedWithoutLoginAction -= UpdateLanguageFromDb;

    }

    void Start()
    {
        LoadingBar.SetActive(false);
        Cursor.lockState = CursorLockMode.None;

        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            AuthPanel.SetActive(true);
        }
        else
        {
            onLaunchScreenOpenedWithoutLoginAction?.Invoke();
        }
    }

    public void StartSingleGame()
    {
        LaunchManager.instance.StartCoroutine(nameof(LaunchManager.instance.CreateSingleRoom));
        startMatchmakingButton.GetComponent<Button>().interactable = false;
        cancelMatcmakingButton.GetComponent<Button>().interactable = false;
    }

    public void CreateMatchmakingTicket()
    {
        LaunchManager.instance.CreateMatchmakingTicket();
        matchmakingTimer.gameObject.SetActive(true);
        cancelMatcmakingButton.SetActive(true);
        startMatchmakingButton.SetActive(false);
        startSingleGameButton.GetComponent<Button>().interactable = false;
    }

    public void CancelMatchmakingTicket()
    {
        LaunchManager.instance.CancelMatchmakingTicket();
        matchmakingTimer.gameObject.SetActive(false);
        cancelMatcmakingButton.SetActive(false);
        startMatchmakingButton.SetActive(true);
        startSingleGameButton.GetComponent<Button>().interactable = true;
    }

    public void CloseAuthPanel()
    {
        LoadingBar.SetActive(false);
        AuthPanel.SetActive(false);
    }

    public void UpdateCollectedWoodText()
    {
        playerWoodCount.text = PlayerProperty.instance.collectedWoodCount.ToString();
    }

    private void UpdateLaunchCountText()
    {
        launchCountText.text = "Launch Count From Azure: " + AzureFunction.launchCount;
    }

    public void OpenProfilePanel()
    {
        playerDisplayName.text = PlayfabManager.displayName;
        playerNamePanel.SetActive(true);
        WoodPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void UpdateLanguageFromDb()
    {
        if (PlayerProperty.instance.characterLanguageIndex == 0) womanToggle.isOn = true;
        else manToggle.isOn = true;
    }

    public void ChangeLanguageOfCharacters(int index)
    {
        PlayerProperty.instance.characterLanguageIndex = index;
    }

}
