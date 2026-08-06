using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text dialogueText;

    public Image leftPortrait;
    public Image middlePortrait;

    int currentIndex; //Guarda a linha de dialogo
    Dialogue currentDialogue; //mostra os dialogos de acordo com a linha

    void StartDialogue(Dialogue dialogue)
    {
        currentDialogue = dialogue;
        currentIndex = 0;

        ShowLine();
    }

    public void NexLine() //passar o dialogo
    {
        currentIndex++;

        if (currentIndex >= currentDialogue.lines.Length)
        {
            EndDialogue();
            return;
        }

        ShowLine();
    }

    public void ShowLine() //Mostrar o dialogo
    {
        DialogueLine line = currentDialogue.lines[currentIndex];

        nameText.text = line.speakerName;

        Sprite sprite = GetExpression(line.speakerName, line.expression);

        leftPortrait.sprite = sprite;

    }

    public void EndDialogue()
    {

    }
}
//Como posso fazer um sistema de dialogo na unity onde possui o dialogo, nome da pessoa que está falando, moldura do player e do npc junto de como poderia mudar suas expressoes conforme falam