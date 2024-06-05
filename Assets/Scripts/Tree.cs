using System.Collections;
using System.Collections.Generic;
using EPOOutline;
using Photon.Pun;
using TMPro;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.AI;

public class Tree : MonoBehaviourPun
{
    [Header("Tree's Property")]
    [SerializeField] public int hP = 100;
    [SerializeField] public Rigidbody rb;
    [SerializeField] public NavMeshObstacle navMeshObstacle;
    [SerializeField] public BoxCollider boxColl;
    [SerializeField] public Canvas woodCountCanvas;
    [SerializeField] private Color transparentColor;
    private Outlinable outline;
    private MeshRenderer meshRenderer;

    [Header("FromDirectory")]

    [Tooltip("Wood Prefab")]
    [SerializeField] private TMP_Text woodCountText;
    [Tooltip("Transparent Material")]
    [SerializeField] public Material transparentMaterial;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boxColl = GetComponent<BoxCollider>();
        outline = GetComponent<Outlinable>();
        navMeshObstacle = GetComponent<NavMeshObstacle>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public IEnumerator FlashOutline()
    {
        outline.enabled = true;
        yield return new WaitForSeconds(0.05f);
        outline.enabled = false;
    }

    public void DropTree()
    {
        boxColl.isTrigger = false;
        rb.isKinematic = false;
        navMeshObstacle.enabled = false;
        meshRenderer.material = transparentMaterial;
        meshRenderer.material.color = transparentColor;

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
        woodCount.color = teamColor;
        if (woodCount.gameObject != null) Destroy(woodCount.gameObject, 1f);
    }

    public void DestroyTree()
    {
        if (gameObject != null) PhotonNetwork.Destroy(gameObject);
    }


}
