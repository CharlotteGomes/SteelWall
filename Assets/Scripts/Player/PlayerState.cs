using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public abstract class PlayerState
{
    public PlayerController controller;
    public abstract void Execute();
    public PlayerState(PlayerController c)
    {
        controller = c;
        controller.ChangeState(this);
    }
}

public class PlayerWalk : PlayerState
{
    float smooth_value = 0.3f;
    bool can_move = true;
    public PlayerWalk(PlayerController c) : base(c) { can_move = true; }

    public override void Execute()
    {
        InputManager.GetInputs();
        if (CheckHorizontals()){ MoveTo(new Vector2(InputManager.horizontal, 0)); return; }
        if (CheckVerticals()) { MoveTo(new Vector2(0, InputManager.vertical)); return; }
    }

    bool CheckHorizontals()
    {
        if(InputManager.horizontal != 0) { return true; }
        return false;
    }

    bool CheckVerticals()
    {
        if (InputManager.vertical != 0) { return true; }
        return false;
    }

    void MoveTo(Vector2 dir)
    {

    }
}
