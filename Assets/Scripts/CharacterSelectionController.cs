using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class CharacterSelectionController : MonoBehaviour
{
    public static CharacterSelectionController instance;

    [Header("LayerMasks")]
    [SerializeField] private LayerMask selectableLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask treeLayer;
    [SerializeField] private LayerMask uiLayer;

    [Header("Camera")]
    [SerializeField] private CameraController cameraController;

    [Header("ListOfCharacters")]
    public List<Character> selectedCharacters = new List<Character>();

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
    }


    void Update()
    {
        HandleSelection(); // Eğer kamera hareketi yapılmıyorsa seçim ve hareket işlemlerini yap
    }

    void HandleSelection()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (IsPointerOverUIObject())
            {
                Debug.Log("PointerOverUIObject");
                return;
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;


            if (Physics.Raycast(ray, out hit, Mathf.Infinity, selectableLayer))
            {
                Character selectedCharacter = hit.collider.GetComponent<Character>();
                selectedCharacter.SelectCharacter();
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, treeLayer) && !cameraController.IsPanning())
            {
                StartCoroutine(hit.transform.GetComponent<Tree>().FlashOutline());
                foreach (Character character in selectedCharacters)
                {
                    character.GoToGivenTree(hit);
                }
                ClearSelectedCharacters();
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer) && !cameraController.IsPanning())
            {
                foreach (Character character in selectedCharacters)
                {
                    character.GoToGivenPoint(hit);
                }
                ClearSelectedCharacters();
            }
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.layer == 10)
            {
                return true;
            }
        }

        return false;
    }


    public void PrintAgent()
    {
        foreach (Character character in selectedCharacters)
        {
            print("stoppingDistance: " + character.agent.stoppingDistance);
            print("remainingDistance: " + character.agent.remainingDistance);
            print("agent.hasPath: " + character.agent.hasPath);
        }
    }



    void ClearSelectedCharacters()
    {
        List<Character> charactersToRemove = new List<Character>();

        foreach (Character character in selectedCharacters)
        {
            if (!character.selectedWhileWalking)
            {
                charactersToRemove.Add(character);
            }
        }

        foreach (Character character in charactersToRemove)
        {
            character.ChangeOutline(false);
            selectedCharacters.Remove(character);
        }
    }
}
