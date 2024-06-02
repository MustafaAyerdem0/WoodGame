using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LumberingState : IState
{
    public void EnterState(Character character)
    {
        character.animator.SetBool("Lumbering", true);
        character.isLumbering = true;
        character.StartLumbering();
        Debug.Log("Lumbering");

    }

    public void UpdateState(Character character)
    {

    }

    public void ExitState(Character character)
    {
        character.animator.SetBool("Lumbering", false);
        character.StopLumbering();
        character.isLumbering = false;
        character.SetTreeDestinationBool(false);
    }
}
