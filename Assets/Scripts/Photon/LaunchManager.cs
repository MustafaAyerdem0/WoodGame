using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.MultiplayerModels;
using Unity.VisualScripting;

public class LaunchManager : MonoBehaviourPunCallbacks
{

    public static LaunchManager instance;

    #region Unity Methods

    //first method that is executed
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance != null) Destroy(gameObject);
        else instance = this;



        PhotonNetwork.AutomaticallySyncScene = true;
    }




    #endregion


    #region  Public Methods

    public void ConnectToPhotonServer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            CanvasManager.instance.ConnectionStatusPanel.SetActive(true);


        }
    }


    public void joinRandomRoom()
    {
        //Photon will try to find a room to join if there is at least one room
        CanvasManager.instance.ConnectionStatusPanel.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
    }

    #endregion




    public void CreateMatchmakingTicket()
    {
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
            GiveUpAfterSeconds = 60
        };

        PlayFabMultiplayerAPI.CreateMatchmakingTicket(request, OnMatchmakingTicketCreated, OnPlayFabError);
    }

    private void OnMatchmakingTicketCreated(CreateMatchmakingTicketResult result)
    {
        Debug.Log("Matchmaking ticket created with ID: " + result.TicketId);
        StartCoroutine(PollMatchmakingTicket(result.TicketId));
    }

    private IEnumerator PollMatchmakingTicket(string ticketId)
    {
        bool loop = true;
        while (loop)
        {
            var request = new GetMatchmakingTicketRequest
            {
                TicketId = ticketId,
                QueueName = "WoodCollectionMatchmakingQueue"
            };

            PlayFabMultiplayerAPI.GetMatchmakingTicket(request, result =>
            {
                if (result.Status == "Matched")
                {
                    Debug.Log("Match found!");
                    string matchId = result.MatchId; // Eşleşen match ID'sini alın
                    PhotonNetwork.ConnectUsingSettings();
                    PhotonNetwork.NickName = PlayFabSettings.staticPlayer.EntityId;
                    PhotonNetwork.GameVersion = "1";
                    StartCoroutine(JoinPhotonRoom(matchId));
                    loop = false;

                }
            }, OnPlayFabError);

            yield return new WaitForSeconds(5);
        }
    }

    private void OnPlayFabError(PlayFabError error)
    {
        Debug.LogError("PlayFab error: " + error.GenerateErrorReport());
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
        Invoke(nameof(LoadGameScene), 5);
        // Odaya katıldığınızda yapılacak işlemler
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning("Failed to join room: " + message);
        // Odaya katılma başarısız olursa yapılacak işlemler
    }

    public void LoadGameScene()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("TestScene");
    }



    #region Photon Callbacks

    //this method calls when we connected to  photon servers.
    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.NickName + " Connected to Photon Server");
        //CanvasManager.instance.LobbyPanel.SetActive(true);
        CanvasManager.instance.ConnectionStatusPanel.SetActive(false);
        CanvasManager.instance.EnterNamePanel.SetActive(false);
    }

    //this method will call when we have the internet connection or before onConnectedToMaster method
    public override void OnConnected()
    {
        Debug.Log("Connected to Internet!");
    }

    //call this method if the user failed to join a random room
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log(message);
        CreateAndJoinRoom();
    }

    // public override void OnJoinedRoom()
    // {
    //     //who joines to which room, we will see
    //     Debug.Log(PhotonNetwork.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name);
    //     PhotonNetwork.LoadLevel("GameScene");
    // }

    //it is called when a remote player joins the room that we are in.
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount);
    }
    #endregion




    #region Private methods

    void CreateAndJoinRoom()
    {
        string randomRoomName = "Room" + Random.Range(0, 10000);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 20;

        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
    }

    #endregion
}
