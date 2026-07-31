using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("Informações")]
    public string characterName;

    [Header("Expressões")]
    public Sprite normal;
    public Sprite happy;
    public Sprite sad;
    public Sprite angry;
}
