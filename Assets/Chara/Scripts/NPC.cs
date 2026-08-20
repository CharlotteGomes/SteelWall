using UnityEngine;

public class NPC : MonoBehaviour
{

    //iniciador do dialogo
    public Dialogue dialogue;
    public DialogueManager manager;

    void Start() 
    {
        manager.StartDialogue(dialogue);
    }

    
}
