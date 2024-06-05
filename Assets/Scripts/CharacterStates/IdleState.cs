using UnityEngine;

public class IdleState : IState
{
    public void EnterState(Character character)
    {
        character.animator.SetBool("Idle", true);
    }

    public void ExitState(Character character)
    {
        character.animator.SetBool("Idle", false);
    }
}
