using UnityEngine;

[CreateAssetMenu(fileName = "DamageModifier", menuName = "Scriptable Objects/SkillSO/Effects/Status Effect/DamageBuff")]
public class DamageBuffEffect : ApplyStatusEffect
{
    public int value;

    public override void Execute(BattleChar user, BattleResult result)
    {
        if (self)
        {
            user.GiveStatus(new DamageModifierEffect(user, user, sfSO, stacks, value, resist, RuntimeDuration));
            return;
        }

        foreach (BattleChar target in result.hit)
            user.GiveStatus(new DamageModifierEffect(user, target, sfSO, stacks, value, resist, RuntimeDuration));
    }
}