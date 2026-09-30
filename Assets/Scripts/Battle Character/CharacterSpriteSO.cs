using UnityEngine;

public enum  BattleSprites
{
    Idle,
    Hurt,
    buff,
}

[CreateAssetMenu(fileName = "BattleCharSO", menuName = "Scriptable Objects/Char/Sprites")]
public class CharacterSpriteSO : ScriptableObject
{
    public Sprite idle;
    public Sprite hurt;
    public Sprite buff;
}
