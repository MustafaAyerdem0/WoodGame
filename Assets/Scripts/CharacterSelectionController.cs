using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class CharacterSelectionController : MonoBehaviour
{
    public static CharacterSelectionController instance;
    private GameObject trackMarker;

    [Header("LayerMasks")]
    [SerializeField] private LayerMask selectableLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask treeLayer;
    [SerializeField] private LayerMask uiLayer;

    [Header("Camera")]
    [SerializeField] private CameraController cameraController;

    [Header("ListOfCharacters")]
    public List<Character> selectedCharacters = new List<Character>();
    [Header("Prefab")]
    [SerializeField] private GameObject trackMarkerPrefab;


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
        HandleSelection();
    }

    void HandleSelection()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (IsPointerOverUIObject()) //Check if Pointer Over UI Object
            {
                return;
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, selectableLayer)) // Character selection
            {
                Character selectedCharacter = hit.collider.GetComponent<Character>();
                selectedCharacter.SelectCharacter();
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, treeLayer) && !cameraController.IsPanning()) // Tree selection
            {
                StartCoroutine(hit.transform.GetComponent<Tree>().FlashOutline());
                foreach (Character character in selectedCharacters)
                {
                    character.GoToGivenTree(hit);
                }
                ClearSelectedCharacters();

            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer) && !cameraController.IsPanning()) // Select destination
            {
                InstantiateTrackMarker(hit);
                foreach (Character character in selectedCharacters)
                {
                    character.GoToGivenPoint(hit);
                }
                ClearSelectedCharacters();
            }
        }
    }

    public void InstantiateTrackMarker(RaycastHit hit) // Create Track Marker
    {
        trackMarker = Instantiate(trackMarkerPrefab);
        trackMarker.transform.position = hit.point + hit.normal * 0.01f;
        Destroy(trackMarker, 1.5f);
    }

    private bool IsPointerOverUIObject() //Check if Pointer Over UI Object
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
    void ClearSelectedCharacters() // Clear all characters from the list except those selected when they already have a destination
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
