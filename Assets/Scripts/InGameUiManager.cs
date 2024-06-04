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
    [Header("Wood")]
    [SerializeField] private TMP_Text woodCountText;
    [SerializeField] private Animation countScaleAnimation;

    [Header("CharacterProfileVideo")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private VideoClip greenCharacter, redCharacter;

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
        woodCountText.text = PlayerProperty.instance?.collectedWoodCount.ToString();
    }


    public void UpdateText()
    {
        countScaleAnimation.Play();
        woodCountText.text = PlayerProperty.instance?.collectedWoodCount.ToString();
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
