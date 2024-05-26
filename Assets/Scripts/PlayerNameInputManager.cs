using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerNameInputManager : MonoBehaviour
{
    //this method will be called once we enter a name to input field
    public void SetPlayerName(String playername)
    {
        if (string.IsNullOrEmpty(playername))
        {
            Debug.Log("Player name is empty!");
            return;
        }

        PhotonNetwork.NickName = playername;
    }
}
