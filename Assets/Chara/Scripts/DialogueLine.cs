using UnityEngine;
using System;
[Serializable]
public class DialogueLine 
{

    public CharacterData speaker;
    [TextArea]
    public string text;
    public Expression expression;

    public bool isPlayer;

}
