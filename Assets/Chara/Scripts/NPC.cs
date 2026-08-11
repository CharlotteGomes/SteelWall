using UnityEngine;

public class NPC : MonoBehaviour
{

    //iniciador do dialogo
    public Dialogue dialogue;
    public DialogueManager manager;

    public void StartDialogue() 
    {
        manager.StartDialogue(dialogue);
    }

    
}
