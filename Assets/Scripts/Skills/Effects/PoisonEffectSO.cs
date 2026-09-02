using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "PoisonEffect", menuName = "Scriptable Objects/SkillSO/Effects/Status Effect/Poison")]
public class PoisonEffectSO : ApplyStatusEffect
{
    public int tier;

    public override void Execute(BattleContext ctx)
    {
        foreach(BattleChar target in ctx.Target)
        {
            PoisonEffect poison = new PoisonEffect(ctx.Self, target, sfSO, tier);
            ctx.Self.GiveStatus(poison);        
        }
    }
}
