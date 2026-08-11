using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "Scriptable Objects/Dialogue")]
public class Dialogue : ScriptableObject
{
    //Guarda as linhas de dialogo em uma lista ae
    public DialogueLine[] lines;
}
