using System.Reflection;
using UnityEngine;
using System.Collections;

public abstract class BattleChar : MonoBehaviour
{
    [SerializeField] protected BattleCharSO battle_char;
    [SerializeField] protected BattleContext ctx;
    [SerializeField] protected SkillSO[] skills;
    [SerializeField] protected SkillSO skill_current;
    [SerializeField] Animator animator;
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
    public int Stats(Stats stats)
    {
        switch (stats) 
        {
            case Stats.Strength:
                return strength;
            case Stats.Dexterity:
                return dexterity;
            case Stats.Constitution:
                return constitution;
            case Stats.Mind:
                return mind;
            case Stats.Int:
                return intelligence;
            default:
                return 0;

        }
    }//Function for make something stats more easy to access
    public void SetBattleChar(BattleCharSO battle_char)
    {
        this.battle_char = battle_char; 
    }
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
            float dmg = (float)amount * (1 - (float)Resistences(type) / 100);
            if(dmg > 0)
            {
                Debug.Log($"Tomou {dmg} de dano");
                life_current = Mathf.Clamp(life_current -= (int)dmg, 0, 1000);
            }
        }
    }

    public virtual IEnumerator StartSkill()
    {
        if (skill_current == null) yield break;
        //animator.Play(skill_current.clip.name);
    }

    public virtual void ExecuteSkillsEffects()
    {
        if (skill_current == null) return;
        foreach(SkillEffectSO f in skill_current.effects)
        {
            f.Execute(ctx);
        }
    }

    
}
