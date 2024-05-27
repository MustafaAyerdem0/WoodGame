using UnityEngine;

public interface IState
{
    void EnterState(Character character);
    void UpdateState(Character character);
    void ExitState(Character character);
}


