using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("Portraits")]
    [SerializeField] private Image playerPortrait;
    [SerializeField] private Image npcPortrait;

    [Header("Text")]
    [SerializeField] private TMP_Text speakerName;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Dialogue")]
    [SerializeField] private DialogueData dialogue;

    private int currentLine;

    public void NextLine()
    {
        if (currentLine >= dialogue.lines.Length)
        {
            return;

            DialogueLine line = dialogue.lines[currentLine];

            speakerName.text = line.character.characterName;
            dialogueText.text = line.text;

            Sprite expression = GetExpression(line.character, line.expression);

            if (line.speaker == SpeakerType.Player)
            {
                playerPortrait.sprite = expression;

                playerPortrait.color = new Color(1, 1, 1, 0.4f);
            }

            currentLine++;
        }
    }

    private Sprite GetExpression(CharacterData character, Expression expression)
    {
        switch (expression)
        {
            case Expression.Normal:
                return character.normal;

            case Expression.Happy:
                return character.happy;

            case Expression.Sad:
                return character.sad;

            case Expression.Angry:
                return character.angry;

            default:
                return character.normal;
        }
    }
}
