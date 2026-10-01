using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "PoisonEffect", menuName = "Scriptable Objects/SkillSO/Effects/Status Effect/Poison")]
public class PoisonApplyEffectSO : ApplyStatusEffect
{
    public int tier;

    public override void Execute(BattleChar user, BattleResult result)
    {
        if (self)
        {
            PoisonEffect poison = new PoisonEffect(user, user, sfSO, tier, name);
            user.GiveStatus(poison);
            return;
        }

        foreach(BattleChar target in result.hit)
        {
            PoisonEffect poison = new PoisonEffect(user, target, sfSO, tier, name);
            user.GiveStatus(poison);        
        }
    }
}
