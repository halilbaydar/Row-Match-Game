using UnityEngine;
using System.Collections;

public abstract class BaseState
{
    public State StateName;
    public BoardStateMachine boardStateMachine;

    public BaseState(State name, BoardStateMachine boardStateMachine)
    {
        this.StateName = name;
        this.boardStateMachine = boardStateMachine;
    }

    public virtual State GetState()
    {
        return StateName;
    }

    public virtual void Enter()
    {
        //Debug.Log("Now entering: " + StateName);
    }

    public virtual void UpdateState() { }
    public virtual void Exit() { }
}
