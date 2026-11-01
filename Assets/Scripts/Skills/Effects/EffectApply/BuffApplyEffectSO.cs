using UnityEngine;

[CreateAssetMenu(fileName = "Buff", menuName = "Scriptable Objects/SkillSO/Effects/Status Effect/BuffEffect")]
public class BuffApplyEffectSO : ApplyStatusEffect
{
    public StatsModifierSO so;

    public override void Execute(BattleChar user, BattleResult result)
    {
        if (self)
        {
            user.GiveStatus(new StatModifierEffect(user, user, stacks, sfSO, so, resist, RuntimeDuration));
            return;
        }

        foreach (BattleChar target in result.hit)
            user.GiveStatus(new StatModifierEffect(user, target, stacks, sfSO, so, resist, RuntimeDuration));
    }
}