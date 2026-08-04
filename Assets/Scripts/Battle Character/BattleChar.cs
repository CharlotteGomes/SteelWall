using System.Reflection;
using UnityEngine;

public abstract class BattleChar : MonoBehaviour
{
    [SerializeField] BattleCharSO battle_char;
    [SerializeField] protected int strength, dexterity, constitution, intelligence, mind;

    public void InitStats()
    {
        strength = battle_char.Str;
        dexterity = battle_char.Dex;
        constitution = battle_char.Cons;
        intelligence = battle_char.Int;
        mind = battle_char.Mnd;

    }
}
