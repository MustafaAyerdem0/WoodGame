using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Character : MonoBehaviourPun
{
    //[HideInInspector]
    public NavMeshObstacle targetTreeObstacle;
    private Tree targetTree;
    Outline outline;

    [HideInInspector]
    public NavMeshAgent agent;

    [SerializeField]
    private IState currentState;

    public Animator animator;
    private bool treeDestinationSet;
    public bool isLumbering;

    public LayerMask playersLayerMask;

    Color teamColor;

    Coroutine lumberingCoroutine;
    public Image miniMapImage;


    private void Awake()
    {
        outline = GetComponent<Outline>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        currentState = new IdleState();
        currentState.EnterState(this);
    }



    private void Start()
    {
        if (!PhotonNetwork.IsConnected) return;
        if (!photonView.IsMine)
        {
            gameObject.layer = 8;
        }
        ChangeColor();
    }

    private void Update()
    {
        currentState.UpdateState(this);
    }

    public void ChangeState(IState state)
    {
        currentState.ExitState(this);
        currentState = state;
        currentState.EnterState(this);
    }

    public IState GetState()
    {
        return currentState;
    }

    public void StartLumbering()
    {
        if (photonView.IsMine)
        {
            lumberingCoroutine = StartCoroutine(LumberTree());
        }
    }

    public void StopLumbering()
    {
        if (photonView.IsMine && lumberingCoroutine != null)
        {
            StopCoroutine(lumberingCoroutine);
        }
    }

    public void ChangeOutline(bool active)
    {
        outline.enabled = active;
    }

    public void SetTreeDestination(Vector3 destination)
    {
        treeDestinationSet = true;
        agent.SetDestination(destination);
    }

    public void SetTreeDestinationBool(bool active)
    {
        treeDestinationSet = active;
    }

    public bool IsTreeDestinationSet()
    {
        return treeDestinationSet;
    }

    public IEnumerator LumberTree()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (isLumbering && targetTreeObstacle != null)
            {
                targetTree = targetTreeObstacle.GetComponent<Tree>();
                if (targetTree.hP > 0)
                {
                    PlayerProperty.instance.collectedWoodCount += 10;
                    InGameUiManager.instance.UpdateText();

                }
                CutTree(targetTree.photonView.ViewID);
            }

        }
    }

    public void CutTree(int targetTreeViewId)
    {
        photonView.RPC("RPC_CutTree", RpcTarget.All, targetTreeViewId);
    }

    [PunRPC]
    void RPC_CutTree(int targetTreeViewId)
    {
        Tree targetTree = PhotonView.Find(targetTreeViewId).GetComponent<Tree>();
        print("hp--");
        if (targetTree.hP >= 10)
        {
            targetTree.hP -= 10;
            targetTree.SpawnWoodText(teamColor);

            if (targetTree.hP <= 0)
            {
                targetTree.boxColl.isTrigger = false;
                targetTree.boxColl.excludeLayers = playersLayerMask;
                targetTree.rb.isKinematic = false;
                targetTree.navMeshObstacle.enabled = false;
                Color color = Color.white;
                color.a = 0.51f;
                targetTree.GetComponent<Renderer>().material.color = color;
                targetTree.DropTree();

                ChangeState(new IdleState());

            }
        }
        else if (targetTree.hP <= 0)
        {
            ChangeState(new IdleState());
        }
    }

    public void ChangeColor()
    {
        photonView.RPC("RPC_ChangeColor", RpcTarget.All);
    }

    [PunRPC]
    void RPC_ChangeColor()
    {
        teamColor = GameManager.instance.colors[gameObject.GetPhotonView().OwnerActorNr - 1];
        transform.GetChild(1).GetComponent<Renderer>().material.color = teamColor;
        miniMapImage.color = teamColor;
        if (photonView.IsMine) InGameUiManager.instance.SetCharactersProfile(teamColor);
    }
}
