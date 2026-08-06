using UnityEngine;
using System;
[Serializable]
public class DialogueLine 
{
    
    public string speakerName;
    [TextArea]
    public string text;
    public Sprite portrait;
    public Expression expression;

    public bool isPlayer;

}
