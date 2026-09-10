using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using Unity.VectorGraphics;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    private Coroutine typingCoroutine;
    private bool dialogueActive;
    public TMP_Text nameText;
    public TMP_Text dialogueText;

    public Image leftPortrait;
    public Image middlePortrait;

    int currentIndex; //Guarda a linha de dialogo
    Dialogue currentDialogue; //mostra os dialogos de acordo com a linha

    IEnumerator TypeText(string text) //Aparição das letras do dialogo aos poucos
    {
        dialogueText.text = "";

        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void Start() 
    {
        if (GM.instance.currentDialogue != null)
        {
            StartDialogue(GM.instance.currentDialogue);
        } 
    }

    void Update() 
    {
        if (!dialogueActive)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            NextLine();
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        dialogueActive = true;
        currentDialogue = dialogue;
        currentIndex = 0;

        ShowLine();
    }

    public void NextLine() //passar o dialogo
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
        dialogueText.text = "TESTE DO DIÁLOGO!";
        nameText.text = "TESTE";

        Debug.Log("ShowLine está funcionando!");



        DialogueLine line = currentDialogue.lines[currentIndex]; //linha de dialogo

        nameText.text = line.speaker.characterName; //nome de quem fala
                                                   
        Sprite sprite = line.speaker.GetExpression(line.expression); //expressão atual

        if (line.player) //Se o player estiver falando ficará com a cor inteira, caso contrario ficara acinzentado. Vice versa ao NPC
        {
            middlePortrait.sprite = sprite;

            middlePortrait.color = Color.white;
            leftPortrait.color = Color.gray;
        }
        else
        {
            leftPortrait.sprite = sprite;

            leftPortrait.color = Color.white;
            middlePortrait.color = Color.gray;
        }

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeText(line.text));
    }

    public void EndDialogue()
    {
        dialogueActive = false;
        currentDialogue = null;

        SceneManager.LoadScene(GM.instance.previousScene);

        Debug.Log("Cabou");
    }
}