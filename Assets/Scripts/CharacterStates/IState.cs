using UnityEngine;

public interface IState
{
    void EnterState(Character character);
    void ExitState(Character character);
}


