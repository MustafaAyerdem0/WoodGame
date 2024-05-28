using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LumberingState : IState
{
    public void EnterState(Character character)
    {
        character.animator.SetBool("Lumbering", true);
    }

    public void UpdateState(Character character)
    {

    }

    public void ExitState(Character character)
    {
        character.animator.SetBool("Lumbering", false);
        character.SetTreeDestinationBool(false);
    }
}
