using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Scriptable Objects/SkillSO/Effects/Status Effect/DamageBuff")]
public class DamageBuffEffect : ApplyStatusEffect
{
    public int value;
    public override void Execute(BattleChar user, BattleResult result)
    {
        if (self)
        {
            DamageModifierEffect mod = new DamageModifierEffect(user, user, sfSO, value, name);
            user.GiveStatus(mod);
            return;
        }

        foreach (BattleChar target in result.hit)
        {

            DamageModifierEffect mod = new DamageModifierEffect(user, target, sfSO, value, name);
            user.GiveStatus(mod);
        }
    }
}
