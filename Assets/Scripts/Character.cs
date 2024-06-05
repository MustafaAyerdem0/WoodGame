using System.Collections;
using System.Collections.Generic;
using EPOOutline;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Character : MonoBehaviourPun
{
    [SerializeField] private Image miniMapImage;
    [HideInInspector] private NavMeshObstacle targetTreeObstacle;
    [HideInInspector] public Animator animator;
    [HideInInspector] public CharacterProfile characterProfile;
    [HideInInspector] public int CharacterNumber;
    [HideInInspector] public bool isLumbering;
    [HideInInspector] public bool selectedWhileWalking;
    private bool treeDestinationSet;
    private NavMeshAgent agent;
    private IState currentState;
    private Tree targetTree;
    private Outlinable outline;
    private Color teamColor;
    private Coroutine lumberingCoroutine;

    private void Awake()
    {
        outline = GetComponent<Outlinable>();
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
        CheckArrival();
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
        characterProfile.outline.enabled = active;
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

    public void SelectCharacter()
    {
        if (CharacterSelectionController.instance.selectedCharacters.Contains(this))
        {
            CharacterSelectionController.instance.selectedCharacters.Remove(this);
            selectedWhileWalking = false;
            ChangeOutline(false);
        }
        else
        {
            CharacterSelectionController.instance.selectedCharacters.Add(this);
            if (agent.hasPath) selectedWhileWalking = true;
            AudioSourceManager.instance.PlaySelectedSoundEffect(PlayerProperty.instance.characterLanguageIndex);
            ChangeOutline(true);
        }
    }

    public void GoToGivenTree(RaycastHit hit)
    {
        if (agent != null)
        {
            GetComponent<CapsuleCollider>().isTrigger = false;
            if (targetTreeObstacle != null) targetTreeObstacle.enabled = true;
            targetTreeObstacle = hit.transform.GetComponent<Tree>().navMeshObstacle;
            targetTreeObstacle.enabled = false;
            selectedWhileWalking = false;
            ChangeState(new WalkingState());
            agent.isStopped = false;
            SetTreeDestination(hit.transform.position);
            AudioSourceManager.instance.PlayLumberingSoundEffect(PlayerProperty.instance.characterLanguageIndex);
        }
    }

    public void GoToGivenPoint(RaycastHit hit)
    {
        if (agent != null)
        {
            if (targetTreeObstacle != null) targetTreeObstacle.enabled = true;
            GetComponent<CapsuleCollider>().isTrigger = false;
            agent.isStopped = false;
            selectedWhileWalking = false;
            agent.SetDestination(hit.point);
            SetTreeDestinationBool(false);
            ChangeState(new WalkingState());
        }
    }

    public void CheckArrival()
    {
        if (agent != null && !agent.pathPending)
        {
            if (agent.remainingDistance <= 1f)
            {
                if (agent.hasPath)
                {
                    if (!selectedWhileWalking)
                    {
                        CharacterSelectionController.instance.selectedCharacters.Remove(this);
                        ChangeOutline(false);
                    }

                    agent.isStopped = true;
                    agent.ResetPath();

                    // Change state based on the current state
                    if (IsTreeDestinationSet() && targetTreeObstacle != null)
                    {
                        GetComponent<CapsuleCollider>().isTrigger = true;
                        ChangeState(new LumberingState());
                    }
                    else
                    {
                        ChangeState(new IdleState());
                    }
                }
            }
        }
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
                    characterProfile.woodCount += 10;
                    characterProfile.woodCountText.text = characterProfile.woodCount.ToString();
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
