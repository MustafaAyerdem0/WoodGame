using System.Collections;
using System.Collections.Generic;
using Photon.Pun.Demo.Procedural;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class InGameUiManager : MonoBehaviour
{
    public static InGameUiManager instance;

    public TMP_Text woodCountText;

    public Animation countScaleAnimation;

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

    private void Start()
    {
        print("start calisti");
        woodCountText.text = PlayerProperty.instance.collectedWoodCount.ToString();
    }


    public void UpdateText()
    {
        countScaleAnimation.Play();
        woodCountText.text = PlayerProperty.instance.collectedWoodCount.ToString();
    }

    public void LeaveRoom()
    {
        PlayerProperty.instance.SaveData();
        PhotonNetwork.LeaveRoom();
    }

    public void QuitMatch()
    {
        Application.Quit();
    }


}
