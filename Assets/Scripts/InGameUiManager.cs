using System.Collections;
using System.Collections.Generic;
using Photon.Pun.Demo.Procedural;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class InGameUiManager : MonoBehaviour
{
    public static InGameUiManager instance;

    public TMP_Text woodCountText;

    public Animation countScaleAnimation;
    public VideoPlayer videoPlayer;

    public VideoClip greenCharacter, redCharacter;

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

    public void SetCharactersProfile(Color teamColor)
    {
        if (teamColor == GameManager.instance.colors[0]) videoPlayer.clip = greenCharacter;
        else videoPlayer.clip = redCharacter;
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
