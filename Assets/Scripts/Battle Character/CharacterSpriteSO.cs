using UnityEngine;

public enum  BattleSprites
{
    Idle,
    Slash,
    Pierce,
    Bludgeoning,
    Casting,
    Ranged,
    Hurt,
}

[CreateAssetMenu(fileName = "BattleCharSO", menuName = "Scriptable Objects/Char/Sprites")]
public class CharacterSpriteSO : ScriptableObject
{
    public Sprite idle;
    public Sprite slash;
    public Sprite pierce;
    public Sprite bludgeoning;
    public Sprite casting;
    public Sprite ranged;
    public Sprite hurt;

}
