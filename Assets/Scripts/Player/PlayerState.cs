using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    List<IInteractable> interaction_list = new List<IInteractable>(); //That list set the obj to interact
    public PlayerWalk(PlayerController c) : base(c) { can_move = true; } //Mantain the parent constructor, with addictions

    public override void Execute()
    {
        InputManager.GetInputs();
        if(GenericTools.CheckHorizontal()) GM.instance.RunCoroutine(Walk(new Vector2(InputManager.horizontal, 0)));
        if(GenericTools.CheckVertical()) GM.instance.RunCoroutine(Walk(new Vector2(0, InputManager.vertical)));
        if (InputManager.interact) OnInteract();
        AttInteractionList();
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

    public void AttInteractionList() // Update the list for add new objs
    {
        float interact_dist = 3f; //radius to get new objs  
        Collider2D[] hit = Physics2D.OverlapBoxAll(controller.transform.position, Vector2.one * interact_dist, 0); //return all objs around
        if (interaction_list.Count > 0) //if list have one or more objs, re-order list to nearby obj
        {
            interaction_list = interaction_list.OrderBy(a => Vector2.Distance(controller.transform.position, a.GetPosition())).ToList();
            interaction_list.RemoveAll(i => Vector2.Distance(i.GetPosition(), controller.transform.position) > interact_dist);
        }
        foreach (var c in hit)
        {
            IInteractable interactable = c.GetComponent(typeof(IInteractable)) as IInteractable;
            if (interactable != null && !interaction_list.Contains(interactable))
            {
                interaction_list.Add(interactable);
            }
        }

    }

    public void OnInteract()
    {
        if (interaction_list.Count > 0) { interaction_list[0].Interact(); controller.player_state = new PlayerInteract(controller); }
    }
}

public class PlayerInteract : PlayerState 
{
    
    public PlayerInteract(PlayerController c) : base(c) { }
    public override void Execute()
    {
        //Debug
        if (Input.GetKeyDown(KeyCode.J)) { controller.player_state = new PlayerWalk(controller); }
    }
}