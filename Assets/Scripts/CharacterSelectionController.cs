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
        HandleSelection(); // Eğer kamera hareketi yapılmıyorsa seçim ve hareket işlemlerini yap
        CheckArrival();
    }

    void HandleSelection()
    {
        if (Input.GetMouseButtonUp(0))
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
                    AudioSourceManager.instance.PlaySelectedSoundEffect(PlayerProperty.instance.characterLanguageIndex);
                    selectedObject.ChangeOutline(true);
                }
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, treeLayer) && !cameraController.IsPanning())
            {
                StartCoroutine(hit.transform.GetComponent<Tree>().FlashOutline());
                foreach (Character character in selectedCharacters)
                {
                    if (character.agent != null)
                    {
                        character.GetComponent<CapsuleCollider>().isTrigger = false;
                        if (character.targetTreeObstacle != null) character.targetTreeObstacle.enabled = true;
                        character.targetTreeObstacle = hit.transform.GetComponent<Tree>().navMeshObstacle;
                        character.targetTreeObstacle.enabled = false;
                        character.ChangeState(new WalkingState());
                        character.agent.isStopped = false;
                        character.SetTreeDestination(hit.transform.position);
                        AudioSourceManager.instance.PlayLumberingSoundEffect(PlayerProperty.instance.characterLanguageIndex);
                    }
                }
                ClearSelectedCharacters();
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer) && !cameraController.IsPanning())
            {
                foreach (Character character in selectedCharacters)
                {
                    if (character.agent != null)
                    {
                        if (character.targetTreeObstacle != null) character.targetTreeObstacle.enabled = true;
                        character.GetComponent<CapsuleCollider>().isTrigger = false;
                        character.agent.isStopped = false;
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


    void CheckArrival()
    {
        List<Character> arrivedCharacters = new List<Character>();

        foreach (Character character in allCharacters)
        {
            if (character.agent != null && !character.agent.pathPending)
            {
                if (character.agent.remainingDistance <= 1f)
                {
                    if (character.agent.hasPath)
                    {
                        arrivedCharacters.Add(character);
                        selectedCharacters.Remove(character);
                        character.agent.isStopped = true;
                        character.agent.ResetPath();

                        // Change state based on the current state
                        if (character.IsTreeDestinationSet() && character.targetTreeObstacle != null)
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
