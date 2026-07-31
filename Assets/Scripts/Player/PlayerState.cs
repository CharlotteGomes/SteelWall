using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public abstract class PlayerState
{
    public PlayerController controller; //Player Reference
    public abstract void Execute(); //The "Update" of static class
    public PlayerState(PlayerController c) 
    {
        controller = c; //Just for reference the player Objects
        controller.ChangeState(this);
    }
}

public class PlayerWalk : PlayerState //State from Explorer Mode, make player walk
{
    float smooth_value = 0.15f;
    float wait_time = 0.01f;
    bool can_move = true;
    public PlayerWalk(PlayerController c) : base(c) { can_move = true; } //Mantain the parent constructor, with addictions

    public override void Execute()
    {
        InputManager.GetInputs();
        if (GenericTools.CheckHorizontal()) GM.instance.RunCoroutine(Walk(new Vector2(InputManager.horizontal, 0)));
        if (GenericTools.CheckVertical()) GM.instance.RunCoroutine(Walk(new Vector2(0, InputManager.vertical))); 
    }

    IEnumerator Walk(Vector3 dir)
    {
        if (!can_move) yield break;
        can_move = false;
        Vector3 pos = controller.transform.position; //Initial Pos
        Vector3 final_position = pos - dir; // Target position 
        float dist = Vector2.Distance(pos, final_position); //Initial distance (aways 1)

        while(dist > 0.1f) 
        {
            controller.transform.position = Vector2.Lerp(controller.transform.position, final_position, smooth_value); //Make a interpolation "A point to B"
            dist = Vector2.Distance(controller.transform.position, final_position); //Re-check distance
            yield return new WaitForSeconds(wait_time); //Wait 
        }
        controller.transform.position = final_position; //Re-adjust position to grid
        can_move = true; 
    }
}
