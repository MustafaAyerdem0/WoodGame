using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;

public class IdleState : IState
{
    public void EnterState(Character character)
    {
        character.animator.SetTrigger("Idle");
        Debug.Log("idle trigger");
    }

    public void UpdateState(Character character)
    {

    }

    public void ExitState(Character character)
    {
        character.animator.ResetTrigger("Idle");
    }
}
