using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.MultiplayerModels;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LaunchManager : MonoBehaviourPunCallbacks
{
    public static LaunchManager instance;
    string currentTicketId;
    [SerializeField]
    public Coroutine matchmakingCoroutine;
    [SerializeField]
    Coroutine checkCoroutine;
    [SerializeField]
    Coroutine timeoutCoroutine;
    float timer;
    bool isMatchmaking;

    #region Unity Methods

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {

            instance = this;
        }

        DontDestroyOnLoad(gameObject);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        ConnectToPhotonServer();
    }

    private void Update()
    {
        if (isMatchmaking)
        {
            timer += Time.deltaTime;
            CanvasManager.instance.matchmakingTimer.text = "Searching : " + ((int)timer).ToString();
        }
    }

    #endregion

    #region Public Methods

    public void ConnectToPhotonServer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void CreateMatchmakingTicket()
    {
        isMatchmaking = true;
        var request = new CreateMatchmakingTicketRequest
        {
            Creator = new MatchmakingPlayer
            {
                Entity = new PlayFab.MultiplayerModels.EntityKey
                {
                    Id = PlayFabSettings.staticPlayer.EntityId,
                    Type = PlayFabSettings.staticPlayer.EntityType
                }
            },
            QueueName = "WoodCollectionMatchmakingQueue",
            GiveUpAfterSeconds = 22
        };

        PlayFabMultiplayerAPI.CreateMatchmakingTicket(request, OnMatchmakingTicketCreated, OnPlayFabError);
    }

    private void OnMatchmakingTicketCreated(CreateMatchmakingTicketResult result)
    {
        if (checkCoroutine != null) StopCoroutine(checkCoroutine);
        checkCoroutine = null;
        currentTicketId = result.TicketId;
        Debug.Log("Matchmaking ticket created with ID: " + result.TicketId);
        CanvasManager.instance.statusText.text = "Matchmaking ticket created" + "\nLooking for an online player to matchmaking";
        print(timeoutCoroutine);
        timeoutCoroutine = StartCoroutine(MatchmakingTimeoutCoroutine(22));
        matchmakingCoroutine = StartCoroutine(PollMatchmakingTicket(currentTicketId));
    }

    private IEnumerator PollMatchmakingTicket(string ticketId)
    {
        bool loop = true;
        while (loop)
        {
            print("calisiyor");
            var request = new GetMatchmakingTicketRequest
            {
                TicketId = ticketId,
                QueueName = "WoodCollectionMatchmakingQueue"
            };

            PlayFabMultiplayerAPI.GetMatchmakingTicket(request, result =>
            {
                if (result.Status == "Matched")
                {
                    CanvasManager.instance.cancelMatcmakingButton.GetComponent<Button>().interactable = false;
                    Debug.Log("Match found!");
                    CanvasManager.instance.statusText.text = "Match found!";
                    isMatchmaking = false;
                    string matchId = result.MatchId;
                    PhotonNetwork.GameVersion = "1";
                    loop = false;
                    if (timeoutCoroutine != null)
                    {
                        StopCoroutine(timeoutCoroutine);
                        timeoutCoroutine = null;
                    }
                    StartCoroutine(JoinPhotonRoom(matchId));

                    if (matchmakingCoroutine != null)
                    {
                        StopCoroutine(matchmakingCoroutine);
                        matchmakingCoroutine = null;
                    }
                }
            }, OnPlayFabError);

            yield return new WaitForSeconds(3);
        }
    }

    private IEnumerator MatchmakingTimeoutCoroutine(float timeout)
    {
        yield return new WaitForSeconds(timeout - 0.1f);
        print("durdu");
        StopCoroutine(matchmakingCoroutine);
        matchmakingCoroutine = null;
        checkCoroutine = StartCoroutine(CheckTicket());
    }

    IEnumerator CheckTicket()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            GetMatchmakingTicket(currentTicketId);
        }
    }

    void GetMatchmakingTicket(string ticketId)
    {
        var request = new GetMatchmakingTicketRequest
        {
            TicketId = ticketId,
            QueueName = "WoodCollectionMatchmakingQueue"
        };

        PlayFabMultiplayerAPI.GetMatchmakingTicket(request, result =>
        {
            // Ticket'ın durumu "Canceled" ise
            if (result.Status == "Canceled")
            {
                Debug.Log("Matchmaking ticket canceled. Starting new matchmaking...");
                // Yeni matchmaking işlemini başlat
                CreateMatchmakingTicket();
            }
            else
            {
                Debug.Log("Matchmaking ticket status: " + result.Status);
            }
        }, OnPlayFabError);
    }

    private void OnPlayFabError(PlayFabError error)
    {
        Debug.LogError("PlayFab error: " + error.GenerateErrorReport());
        CanvasManager.instance.statusText.text = "PlayFab error: " + error.GenerateErrorReport();
    }

    public void CancelMatchmakingTicket()
    {
        if (!string.IsNullOrEmpty(currentTicketId))
        {
            var request = new CancelMatchmakingTicketRequest
            {
                TicketId = currentTicketId,
                QueueName = "WoodCollectionMatchmakingQueue"
            };

            PlayFabMultiplayerAPI.CancelMatchmakingTicket(request, OnCancelMatchmakingTicketSuccess, OnPlayFabError);
        }
        else
        {
            Debug.LogWarning("No active matchmaking ticket to cancel.");
        }
    }

    private void OnCancelMatchmakingTicketSuccess(CancelMatchmakingTicketResult result)
    {
        StopAllCoroutines();

        Debug.Log("Matchmaking ticket canceled successfully: " + currentTicketId);
        currentTicketId = null;
        CanvasManager.instance.statusText.text = "Matchmaking ticket canceled successfully.";
        isMatchmaking = false;
        timer = 0;
    }


    private IEnumerator JoinPhotonRoom(string roomName)
    {
        while (!PhotonNetwork.IsConnectedAndReady)
        {
            yield return null;
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2; // 1v1 oda için maksimum oyuncu sayısı 2

        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Joined to " + PhotonNetwork.MasterClient.NickName + "'s room");
        CanvasManager.instance.statusText.text = "Joined to " + PhotonNetwork.MasterClient.NickName + "'s room";
        StartCoroutine(LoadGameScene());
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning("Failed to join room: " + message);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom(); // Oyuncuyu Photon odasından çıkar
    }

    public void PrepareMatchmakingScreen()
    {
        CanvasManager.instance.MatchmakingPanel.SetActive(true);
        timer = 0;
        int i = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (i == 0) CanvasManager.instance.player1Name.text = player.NickName;
            if (i == 1) CanvasManager.instance.player2Name.text = PhotonNetwork.CurrentRoom.Players[2].NickName;
            i++;
        }
    }

    public IEnumerator LoadGameScene()
    {
        while (true)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                yield return new WaitForSeconds(1);
                PrepareMatchmakingScreen();
                yield return new WaitForSeconds(2);
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.LoadLevel("TestScene");
                }
                break;
            }
            yield return new WaitForSeconds(1);
        }

    }

    private void StopCoroutines()
    {

        // Stop all coroutines if the object is being destroyed
        if (matchmakingCoroutine != null)
        {
            StopCoroutine(matchmakingCoroutine);
            matchmakingCoroutine = null;
        }
        if (checkCoroutine != null)
        {
            StopCoroutine(checkCoroutine);
            checkCoroutine = null;
        }
        if (timeoutCoroutine != null)
        {
            StopCoroutine(timeoutCoroutine);
            timeoutCoroutine = null;
        }

    }



    #endregion

    #region Photon Callbacks

    public override void OnConnectedToMaster() { }

    public override void OnConnected()
    {
        Debug.Log("Connected to Internet!");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log(message);
        CanvasManager.instance.statusText.text = message;
    }

    public void SetDisplayName(string name)
    {
        PhotonNetwork.NickName = name;
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount);
        CanvasManager.instance.statusText.text = newPlayer.NickName + " joined to room";
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left room successfully");
        //if (SceneManager.GetActiveScene().name != "LaunchScene") SceneManager.LoadScene("LaunchScene");
    }



    #endregion
}
