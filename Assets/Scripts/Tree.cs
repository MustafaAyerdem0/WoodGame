using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class Tree : MonoBehaviourPun
{
    public int hP = 100;
    public Rigidbody rb;
    public NavMeshObstacle navMeshObstacle;

    public BoxCollider boxColl;

    Outline outline;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boxColl = GetComponent<BoxCollider>();
        outline = GetComponent<Outline>();
        navMeshObstacle = GetComponent<NavMeshObstacle>();
    }

    public IEnumerator FlashOutline()
    {
        outline.enabled = true;
        yield return new WaitForSeconds(0.05f);
        outline.enabled = false;
    }

    public void DropTree()
    {
        if (photonView.IsMine)
        {
            print("drop");
            print(GetComponent<PhotonView>().IsMine);
            Vector3 directionToCenter = (Vector3.zero - rb.position).normalized; // to apply force towards the center point
            rb.AddForceAtPosition(new Vector3(directionToCenter.x * 3, 0, directionToCenter.z * 3),
            transform.position + new Vector3(0, 10, 0), ForceMode.VelocityChange);
        }
    }

}
