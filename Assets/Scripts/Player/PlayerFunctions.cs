using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using System.Linq;
using System;
public static class PlayerFunctions
{
    public static PlayerController player_ref;
    public static List<IInteractable> interaction_list = new List<IInteractable>();
    public static void AttInteractionList()
    {
        float interact_dist = 3f;
        RaycastHit2D ray = Physics2D.BoxCast(player_ref.transform.position, Vector2.one * interact_dist, 0, Vector2.zero);
        if (interaction_list.Count > 0)
        {
            interaction_list.OrderBy(a => Vector2.Distance(player_ref.transform.position, a.Transform.position)).ToList();
            foreach (IInteractable i in interaction_list)
            {
                if (Vector2.Distance(i.Transform.position, player_ref.transform.position) > interact_dist){ interaction_list.Remove(i); }
            }

        }
        if (ray.collider != null && ray.collider.TryGetComponent<IInteractable>(out var interaction))
        {
            interaction_list.Add(interaction);
        } 
    }

    public static void OnInteract()
    {
        if(interaction_list.Count > 0) { interaction_list[0].Interact(); }
    }
}
