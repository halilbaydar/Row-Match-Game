using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopState : BaseState
{
    public StopState(BoardStateMachine boardStateMachine) : base(State.STOP, boardStateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void UpdateState()
    {
        boardStateMachine.ChangeState(this);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
