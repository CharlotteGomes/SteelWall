using UnityEngine;

[CreateAssetMenu(fileName = "Bleed", menuName = "Scriptable Objects/SkillSO/Effects/Status Effect/BleedEffect")]
public class BleedApplyEffectSO : ApplyStatusEffect
{
    public override void Execute(BattleChar user, BattleResult result)
    {
        if (self)
        {
            user.GiveStatus(new BleedEffect(user, user, stacks, sfSO, resist, RuntimeDuration, RuntimePotency));
            return;
        }

        foreach (BattleChar target in result.hit)
            user.GiveStatus(new BleedEffect(user, target, stacks, sfSO, resist, RuntimeDuration, RuntimePotency));
    }
}