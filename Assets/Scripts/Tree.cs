using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class Tree : MonoBehaviourPun
{
    public int hP = 100;
    Rigidbody rb;
    NavMeshObstacle navMeshObstacle;

    BoxCollider boxColl;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boxColl = GetComponent<BoxCollider>();
    }
    public void CutTree()
    {
        photonView.RPC("RPC_CutTree", RpcTarget.All);
    }

    [PunRPC]
    void RPC_CutTree()
    {
        if (hP >= 10) hP -= 10;
        if (hP <= 0)
        {
            boxColl.enabled = false;
            rb.AddForce(new Vector3(Random.Range(-10, 10), 0, 5));
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
