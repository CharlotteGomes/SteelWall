using UnityEngine;

[CreateAssetMenu(fileName = "MarkApplyEffect", menuName = "Scriptable Objects/SkillSO/Effects/Status Effect/MarkApplyEffect")]
public class MarkApplyEffect : ApplyStatusEffect
{
    public override void Execute(BattleChar user, BattleResult result)
    {
        if (self)
        {
            user.GiveStatus(new MarkEffect(user, user, sfSO, stacks, resist, ConfiguredDuration, RuntimePotency));
            return;
        }

        foreach (BattleChar target in result.hit)
            user.GiveStatus(new MarkEffect(user, target, sfSO, stacks, resist, ConfiguredDuration, RuntimePotency));
    }
}