using System.Collections;
using System.Collections.Generic;
using EPOOutline;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Tree : MonoBehaviourPun
{
    public int hP = 100;
    public Rigidbody rb;
    public NavMeshObstacle navMeshObstacle;

    public BoxCollider boxColl;

    Outlinable outline;

    public Canvas woodCountCanvas;
    public TMP_Text woodCountText;

    public Material transparentMaterial;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boxColl = GetComponent<BoxCollider>();
        outline = GetComponent<Outlinable>();
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
            Vector3 directionToCenter = (Vector3.zero - rb.position).normalized; // to apply force towards the center point
            rb.AddForceAtPosition(new Vector3(directionToCenter.x * 3, 0, directionToCenter.z * 3),
            transform.position + new Vector3(0, 10, 0), ForceMode.VelocityChange);
            Invoke(nameof(DestroyTree), 3);
        }
    }

    public void SpawnWoodText(Color teamColor)
    {
        TMP_Text woodCount = Instantiate(woodCountText, woodCountCanvas.transform);
        print("spawnlandı");
        woodCount.color = teamColor;
        if (woodCount.gameObject != null) Destroy(woodCount.gameObject, 1f);
    }

    public void DestroyTree()
    {
        if (gameObject != null) PhotonNetwork.Destroy(gameObject);
    }

}
