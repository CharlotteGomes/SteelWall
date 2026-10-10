using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[CreateAssetMenu(fileName = "Penitence", menuName = "Scriptable Objects/SkillSO/Effects/Status Effect/Specify/Penitence")]
public class PenitenceApplyEffectSO : ApplyStatusEffect
{
    public StatsModifierSO damageMaxDown, damageMinDown, dodgeDown, protectUp;
    public override void Execute(BattleChar user, BattleResult result)
    {
        StatsModifierSO[] mods = { damageMaxDown, damageMinDown, dodgeDown, protectUp};
        if (self)
        {
            Penitence mod = new Penitence(user, user, stacks, sfSO, mods, false);
            user.GiveStatus(mod);
            return;
        }

        foreach (BattleChar target in result.hit)
        {

            Penitence mod = new Penitence(user, target, stacks, sfSO, mods);
            user.GiveStatus(mod);
        }
    }
}
