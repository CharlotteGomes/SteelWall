using NUnit.Framework.Internal;
using System.Collections.Generic;
using UnityEngine;
public class PlayerInteract : MonoBehaviour
{
    List<IInteraction> interactionsInRange = new List<IInteraction>();
    IInteraction Interaction;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Interaction = GetNearbyInteraction();
        if(Interaction != null && Input.GetKeyDown(KeyCode.E)) { Interaction.Interact(); }
    }

    IInteraction GetNearbyInteraction()
    {
        IInteraction Nearby = null;
        float minDist = Mathf.Infinity;

        foreach (IInteraction interaction in interactionsInRange)
        {
            float dist = Vector2.Distance(transform.position, interaction.GetTransform().position);

            if (dist < minDist)
            {
                minDist = dist;
                Nearby = interaction;
            }
        }

        return Nearby;

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        IInteraction interaction = collision.GetComponent<IInteraction>();  

        if(interaction != null && !interactionsInRange.Contains(interaction))
        {
            interactionsInRange.Add(interaction);
        } 
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteraction interaction = collision.GetComponent<IInteraction>();

        if (interaction != null && interactionsInRange.Contains(interaction))
        {
            interactionsInRange.Remove(interaction);
        }
    }
}
