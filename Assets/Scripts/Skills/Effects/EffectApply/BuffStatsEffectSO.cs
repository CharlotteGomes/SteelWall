using UnityEngine;

[CreateAssetMenu(fileName = "Buff", menuName = "Scriptable Objects/SkillSO/Effects/Status Effect/BuffEffect")]
public class BuffStatsEffectSO : ApplyStatusEffect
{
    public StatsModifierSO so;
    public override void Execute(BattleChar user, BattleResult result)
    {
        if (self)
        {
            StatModifierEffect mod = new StatModifierEffect(user, user, stacks, sfSO, so, resist);
            user.GiveStatus(mod);
            return;
        }

        foreach (BattleChar target in result.hit)
        {
            StatModifierEffect mod = new StatModifierEffect(user, target, stacks, sfSO, so, resist);
            user.GiveStatus(mod);
        }
    }
}
