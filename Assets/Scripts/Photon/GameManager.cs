using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Unity.Burst.Intrinsics;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] public List<Color> colors = new List<Color>();
    [SerializeField] private List<CharacterProfile> characterProfiles = new List<CharacterProfile>();

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
            go.transform.position = new Vector3(i + ((go.GetPhotonView().OwnerActorNr - 1) * 10), 0, -3f);
            Character character = go.GetComponent<Character>();
            character.CharacterNumber = i;
            characterProfiles[i].character = character;
            character.characterProfile = characterProfiles[i];
        }

    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("LaunchScene");
    }


}