using UnityEngine;

public class InteractDialogue : MonoBehaviour, IInteractable
{
    [SerializeField] private NPC npc;

    public void Interact()
    {
        if (npc != null)
        {
            npc.StartDialogue();
        }
    }
    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
