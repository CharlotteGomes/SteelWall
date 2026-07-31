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

    }

    IEnumerator Walk(Vector3 dir)
    {
        Vector3 final_position = controller.transform.position + dir;
        yield return null;
    }
}
