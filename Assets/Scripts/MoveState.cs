using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : BaseState
{

    public MoveState(BoardStateMachine boardStateMachine) : base(State.MOVE, boardStateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();
        boardStateMachine.board.ApplyEndGame();
        EndGameManager.SINGLETON.FinishTheGame();
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
