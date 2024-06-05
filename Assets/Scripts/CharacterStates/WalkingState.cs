using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingState : IState
{
    public void EnterState(Character character)
    {
        character.animator.SetBool("Walking", true);
    }

    public void ExitState(Character character)
    {
        character.animator.SetBool("Walking", false);
    }


}
