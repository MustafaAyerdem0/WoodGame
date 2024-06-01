using UnityEngine;

public class IdleState : IState
{
    public void EnterState(Character character)
    {
        character.animator.SetBool("Idle", true);
        Debug.Log("idle");
    }

    public void UpdateState(Character character)
    {

    }

    public void ExitState(Character character)
    {
        character.animator.SetBool("Idle", false);
    }
}
