using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DialogueData", menuName = "ScriptableObjects/DialogueData", order = 1)]
public class DialogueData : ScriptableObject
{
    public DialogueLine[] lines;
}

public enum SpeakerType
{
    Player, NPC
}

[System.Serializable]
public class DialogueLine
{
    public SpeakerType speaker;
    public CharacterData character;
    public Expression expression;

    [TextArea(3, 5)]
    public string text;
}
