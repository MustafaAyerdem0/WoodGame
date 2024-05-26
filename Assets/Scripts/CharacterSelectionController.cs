using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class CharacterSelectionController : MonoBehaviour
{
    public LayerMask selectableLayer;
    public LayerMask groundLayer;
    private List<GameObject> selectedCharacters = new List<GameObject>();

    void Update()
    {
        HandleSelection();
        HandleMovement();
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
                GameObject selectedObject = hit.collider.gameObject;

                if (selectedCharacters.Contains(selectedObject))
                {
                    selectedCharacters.Remove(selectedObject);
                    // Seçimi kaldırmak için bir renk değişikliği veya işaretleyici ekleyin
                    selectedObject.transform.GetChild(1).GetComponent<Renderer>().material.color = Color.white;
                }
                else
                {
                    selectedCharacters.Add(selectedObject);
                    // Seçildiğini belirtmek için bir renk değişikliği veya işaretleyici ekleyin
                    selectedObject.transform.GetChild(1).GetComponent<Renderer>().material.color = Color.green;
                }
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                print(hit.transform.name);
                foreach (GameObject character in selectedCharacters)
                {
                    NavMeshAgent agent = character.GetComponent<NavMeshAgent>();
                    if (agent != null)
                    {
                        agent.SetDestination(hit.point);
                    }
                }
            }
        }
    }

    void HandleMovement()
    {
        // Hareket işlemleri, HandleSelection içindeki kodda zaten yapılmaktadır.
    }
}
