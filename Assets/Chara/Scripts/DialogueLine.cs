using UnityEngine;
using System;
[Serializable]
public class DialogueLine 
{

    public CharacterData speaker;
    public Expression expression;
    public bool isPlayer;

    [TextArea]
    public string text;
}
