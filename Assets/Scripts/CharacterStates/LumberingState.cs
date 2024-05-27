using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LumberingState : IState
{
    public void EnterState(Character character)
    {
        character.animator.SetTrigger("Lumbering");
    }

    public void UpdateState(Character character)
    {

    }

    public void ExitState(Character character)
    {
        character.animator.ResetTrigger("Lumbering");
        character.SetTreeDestinationBool(false);
    }
}
