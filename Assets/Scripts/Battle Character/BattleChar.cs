using System.Reflection;
using UnityEngine;

public abstract class BattleChar : MonoBehaviour
{
    [SerializeField] protected BattleCharSO battle_char;
    [SerializeField] protected BattleContext ctx;
    [SerializeField] protected int strength, dexterity, constitution, intelligence, mind;
    [SerializeField] protected int life_max, life_current, mana_max, mana_current;
    public int Resistences(TypeDmg dmg)
    {
        switch (dmg) 
        {
            case TypeDmg.Slashing:
                return (dexterity / 3) + constitution; 
            case TypeDmg.Bludgeoning:
                return (dexterity / 3) + constitution;
            case TypeDmg.Pierce:
                return (dexterity / 3) + constitution;
            case TypeDmg.Fire:
                return (dexterity / 3) + constitution;
            case TypeDmg.Ice:
                return (dexterity / 3) + constitution;
            case TypeDmg.Lightning:
                return (dexterity / 3) + constitution;
            case TypeDmg.Purity:
                return (dexterity / 3) + constitution;
            default:
                return 0;
        }
    }//Function for make something stats more easy to access
    public int Stats(Scaling stats)
    {
        switch (stats) 
        {
            case Scaling.Strength:
                return strength;
            case Scaling.Dexterity:
                return dexterity;
            case Scaling.Constitution:
                return constitution;
            case Scaling.Mind:
                return mind;
            case Scaling.Int:
                return intelligence;
            default:
                return 0;

        }
    }//Function for make something stats more easy to access
    public virtual void InitStats()
    {
        strength = battle_char.Str;
        dexterity = battle_char.Dex;
        constitution = battle_char.Cons;
        intelligence = battle_char.Int;
        mind = battle_char.Mnd;
    }

    public void TakeDamage(TypeDmg type, int amount)
    {
        if(amount > 0)
        {
            int dmg = (amount * 100) / Resistences(type);
            Debug.Log($"Dano Pós Calculo: {dmg} {amount}");
            if(dmg > 0)
            {
                life_current = Mathf.Clamp(life_current -= dmg, 0, 1000);
            }
        }
    }
}
