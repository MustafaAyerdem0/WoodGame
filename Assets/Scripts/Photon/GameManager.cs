using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Unity.Burst.Intrinsics;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject playerPrefab;

    public static GameManager instance;    //singleton design pattern

    public List<Color> colors = new List<Color>();

    public List<Character> characters = new List<Character>();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.AutomaticallySyncScene = true;

            if (playerPrefab != null)
            {
                CreateCharacters();
            }

        }
    }

    private void CreateCharacters()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject go = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
            go.transform.position = new Vector3(i + ((go.GetPhotonView().OwnerActorNr - 1) * 10), 0, 0);
            characters.Add(go.GetComponent<Character>());
        }

    }



    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.NickName + " joined to --" + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " joined to --" + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount);
    }



    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("LaunchScene");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void QuitMatch()
    {
        Application.Quit();
    }
}