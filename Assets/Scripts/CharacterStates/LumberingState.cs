using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LumberingState : IState
{
    public void EnterState(Character character)
    {
        character.animator.SetBool("Lumbering", true);
        character.isLumbering = true;
        Debug.Log("Lumbering");
    }

    public void UpdateState(Character character)
    {

    }

    public void ExitState(Character character)
    {
        character.animator.SetBool("Lumbering", false);
        character.isLumbering = false;
        character.targetTreeObstacle = null;
        character.SetTreeDestinationBool(false);
    }
}
