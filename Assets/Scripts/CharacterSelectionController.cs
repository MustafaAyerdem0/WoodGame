using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

public class CharacterSelectionController : MonoBehaviour
{
    [Header("LayerMasks")]
    [SerializeField] private LayerMask selectableLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask treeLayer;

    [Header("Camera")]
    [SerializeField] private CameraController cameraController;

    [Header("ListOfCharacters")]
    public List<Character> allCharacters = new List<Character>();
    public List<Character> selectedCharacters = new List<Character>();


    void Update()
    {
        if (!cameraController.IsPanning()) // Eğer kamera hareketi yapılmıyorsa seçim ve hareket işlemlerini yap
        {
            HandleSelection();
            HandleMovement();
            //PrintAgent();
            CheckArrival();
        }
    }

    void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, selectableLayer))
            {
                Character selectedObject = hit.collider.GetComponent<Character>();
                if (!allCharacters.Contains(selectedObject)) allCharacters.Add(selectedObject);

                if (selectedCharacters.Contains(selectedObject))
                {
                    selectedCharacters.Remove(selectedObject);
                    selectedObject.ChangeOutline(false);
                }
                else
                {
                    selectedCharacters.Add(selectedObject);
                    selectedObject.ChangeOutline(true);
                }
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, treeLayer))
            {
                StartCoroutine(hit.transform.GetComponent<Tree>().FlashOutline());
                foreach (Character character in selectedCharacters)
                {
                    if (character.agent != null)
                    {
                        character.GetComponent<CapsuleCollider>().isTrigger = false;
                        if (character.targetTreeObstacle != null) character.targetTreeObstacle.enabled = true;
                        character.targetTreeObstacle = hit.transform.GetComponent<NavMeshObstacle>();
                        character.targetTreeObstacle.enabled = false;
                        character.ChangeState(new WalkingState());
                        character.SetTreeDestination(hit.transform.position);
                    }
                }
                ClearSelectedCharacters();
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                foreach (Character character in selectedCharacters)
                {
                    if (character.agent != null)
                    {
                        if (character.targetTreeObstacle != null) character.targetTreeObstacle.enabled = true;
                        character.GetComponent<CapsuleCollider>().isTrigger = false;
                        character.agent.SetDestination(hit.point);
                        character.SetTreeDestinationBool(false);
                        character.ChangeState(new WalkingState());
                    }
                }
                ClearSelectedCharacters();
            }
        }
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

    void HandleMovement()
    {
        // Movement handling is already done in HandleSelection.
    }

    void CheckArrival()
    {
        List<Character> arrivedCharacters = new List<Character>();

        foreach (Character character in allCharacters)
        {
            if (character.agent != null && !character.agent.pathPending)
            {
                if (character.agent.remainingDistance <= 0.6f)
                {
                    if (character.agent.hasPath)
                    {
                        arrivedCharacters.Add(character);
                        selectedCharacters.Remove(character);
                        character.agent.SetDestination(character.transform.position);

                        // Change state based on the current state
                        if (character.IsTreeDestinationSet())
                        {
                            character.GetComponent<CapsuleCollider>().isTrigger = true;
                            character.ChangeState(new LumberingState());
                        }
                        else
                        {
                            character.ChangeState(new IdleState());
                        }
                    }
                }
            }
        }

        foreach (Character character in arrivedCharacters)
        {
            character.ChangeOutline(false);
        }
    }

    void ClearSelectedCharacters()
    {
        foreach (Character character in selectedCharacters)
        {
            character.ChangeOutline(false);
        }
        selectedCharacters.Clear();
    }
}
