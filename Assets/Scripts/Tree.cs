using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class Tree : MonoBehaviourPun
{
    public int hP = 100;
    public Rigidbody rb;
    NavMeshObstacle navMeshObstacle;

    public BoxCollider boxColl;

    Outline outline;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boxColl = GetComponent<BoxCollider>();
        outline = GetComponent<Outline>();
    }

    public IEnumerator FlashOutline()
    {
        outline.enabled = true;
        yield return new WaitForSeconds(0.05f);
        outline.enabled = false;
    }

}
