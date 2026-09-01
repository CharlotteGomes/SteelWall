using UnityEngine;
using System.Collections.Generic;

public abstract class BattleCharSO : ScriptableObject
{
    public string Name;
    public CharacterSpriteSO sprites;
    public int Cons, Str, Dex, Int, Mnd;
    public List<ResistenseData> Resists;
}
