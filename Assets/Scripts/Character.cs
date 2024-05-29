using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviourPun
{

    GameObject targetTree;
    Outline outline;

    [HideInInspector]
    public NavMeshAgent agent;

    [SerializeField]
    private IState currentState;

    public Animator animator;

    private Vector3 treeDestination;
    private bool treeDestinationSet;


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
        ChangeColor();
        if (!photonView.IsMine) gameObject.layer = 8;
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

    public void ChangeOutline(bool active)
    {
        outline.enabled = active;
    }

    public void SetTreeDestination(Vector3 destination)
    {
        treeDestination = destination;
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

    public void ChangeColor()
    {
        photonView.RPC("RPC_ChangeColor", RpcTarget.All);
    }

    [PunRPC]
    void RPC_ChangeColor()
    {
        transform.GetChild(1).GetComponent<Renderer>().material.color = GameManager.instance.colors[gameObject.GetPhotonView().OwnerActorNr - 1];
    }
}
