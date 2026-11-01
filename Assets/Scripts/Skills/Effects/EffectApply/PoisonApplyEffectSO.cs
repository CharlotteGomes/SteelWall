using UnityEngine;

[CreateAssetMenu(fileName = "PoisonEffect", menuName = "Scriptable Objects/SkillSO/Effects/Status Effect/Poison")]
public class PoisonApplyEffectSO : ApplyStatusEffect
{
    public int tier;

    public override void Execute(BattleChar user, BattleResult result)
    {
        if (self)
        {
            user.GiveStatus(new PoisonEffect(user, user, stacks, sfSO, tier, resist, RuntimeDuration, RuntimePotency));
            return;
        }

        foreach (BattleChar target in result.hit)
            user.GiveStatus(new PoisonEffect(user, target, stacks, sfSO, tier, resist, RuntimeDuration, RuntimePotency));
    }
}