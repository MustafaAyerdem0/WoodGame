using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject playerPrefab;

    public static GameManager instance;    //singleton design pattern



    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
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
                int randomPoint = Random.Range(-20, 20);
                PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(randomPoint, 5, randomPoint), Quaternion.identity);
                Debug.Log("*-*-Instantiate");
            }

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
        SceneManager.LoadScene("GameLauncherScene");
    }


    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}