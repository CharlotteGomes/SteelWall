using UnityEngine;


public abstract class BattleCharSO : ScriptableObject
{
    public string Name;
    public CharacterSpriteSO sprites;
    public int Cons, Str, Dex, Int, Mnd;
}
