using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "PoisonEffect", menuName = "Scriptable Objects/SkillSO/Effects/Status Effect/Poison")]
public class PoisonEffectSO : ApplyStatusEffect
{
    public int tier;

    public override void Execute(BattleChar user, BattleResult result)
    {
        foreach(BattleChar target in result.hit)
        {
            PoisonEffect poison = new PoisonEffect(user, target, sfSO, tier);
            user.GiveStatus(poison);        
        }
    }
}
