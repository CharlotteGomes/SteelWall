using UnityEngine;
public enum STATE
{
    DISABLED,
    WAITING,
    TYPING
}


public class DialogueSystem : MonoBehaviour
{

    public DialogueData dialogueData;

    int currentText = 0;
    bool finished = false;

    TypeTextAnimation typeText;

    STATE state;

    void Awake()
    {
        typeText = FindObjectOfType<TypeTextAnimation>();
    }

    void Start()
    {
        state = STATE.DISABLED;
    }

    void Update()
    {
        if (state == STATE.DISABLED) return;

        switch (state)
        {
            case STATE.WAITING:
                Waiting();
                break;
            case STATE.TYPING:
                Typing();
                break;



        }
    }
    public void Next()
    {
        typeText.fullText = dialogueData.talkScript[currentText++].text;

        if (currentText == dialogueData.talkScript.Count) finished = true;
        
        typeText.StartTyping();
        state = STATE.TYPING;
        
         
    }
    void Waiting() 
    {

    }
    void Typing()   
    {

    }
}

