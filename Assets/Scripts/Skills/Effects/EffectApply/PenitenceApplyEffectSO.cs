using UnityEngine;

[CreateAssetMenu(fileName = "Penitence", menuName = "Scriptable Objects/SkillSO/Effects/Status Effect/Specify/Penitence")]
public class PenitenceApplyEffectSO : ApplyStatusEffect
{
    public StatsModifierSO damageMaxDown, damageMinDown, dodgeDown, protectUp;

    public override void Execute(BattleChar user, BattleResult result)
    {
        StatsModifierSO[] mods = { damageMaxDown, damageMinDown, dodgeDown, protectUp };
        if (self)
        {
            user.GiveStatus(new Penitence(user, user, stacks, sfSO, mods, resist, ConfiguredDuration, RuntimePotency));
            return;
        }

        foreach (BattleChar target in result.hit)
            user.GiveStatus(new Penitence(user, target, stacks, sfSO, mods, resist, ConfiguredDuration, RuntimePotency));
    }
}