using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class CharacterSelectionController : MonoBehaviour
{
    public LayerMask selectableLayer;
    public LayerMask groundLayer;
    public LayerMask treeLayer; // New layer mask for trees

    public List<Character> allCharacters = new List<Character>();
    public List<Character> selectedCharacters = new List<Character>();

    void Update()
    {
        HandleSelection();
        HandleMovement();
        //PrintAgent();
        CheckArrival();
    }

    void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;



            if (Physics.Raycast(ray, out hit, Mathf.Infinity, selectableLayer))
            {
                print(hit.transform.name);
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
                print(hit.transform.name);
                foreach (Character character in selectedCharacters)
                {
                    if (character.agent != null)
                    {
                        character.SetTreeDestination(hit.point);
                        character.ChangeState(new WalkingState());
                    }
                }
                ClearSelectedCharacters();
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                print(hit.transform.name);
                foreach (Character character in selectedCharacters)
                {
                    if (character.agent != null)
                    {
                        character.agent.SetDestination(hit.point);
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
                if (character.agent.remainingDistance <= 1f)
                {
                    if (character.agent.hasPath)
                    {
                        arrivedCharacters.Add(character);
                        character.agent.SetDestination(character.transform.position);

                        // Change state based on the current state
                        if (character.IsTreeDestinationSet())
                        {
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