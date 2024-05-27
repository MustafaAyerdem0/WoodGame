using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingState : IState
{
    public void EnterState(Character character)
    {
        character.animator.SetTrigger("Walking");
        Debug.Log("Walking trigger");
    }

    public void UpdateState(Character character)
    {

    }

    public void ExitState(Character character)
    {
        character.animator.ResetTrigger("Walking");
    }


}
