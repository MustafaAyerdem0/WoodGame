using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public GameObject EnterNamePanel;
    public GameObject ConnectionStatusPanel;
    public GameObject LobbyPanel;
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
        EnterNamePanel.SetActive(true);
        ConnectionStatusPanel.SetActive(false);
        //LobbyPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseAuthPanel()
    {
        ConnectionStatusPanel.SetActive(false);
        EnterNamePanel.SetActive(false);
    }

    public void UpdateCollectedWoodText()
    {
        playerWoodCount.text = PlayfabManager.instance.playerProperty.collectedWoodCount.ToString();
    }

}
