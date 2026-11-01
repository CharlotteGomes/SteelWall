using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Scriptable Objects/SkillSO/Effects/Status Effect/HealPerTurn")]
public class HealPerTurnEffectSO : ApplyStatusEffect
{
    public override void Execute(BattleChar user, BattleResult result)
    {
        if (self)
        {
            HealPerTurnEffect heal = new HealPerTurnEffect(
                user, user, stacks, sfSO, resist, RuntimeDuration, RuntimePotency);
            user.GiveStatus(heal);
            return;
        }

        foreach (BattleChar target in result.hit)
        {

            HealPerTurnEffect heal = new HealPerTurnEffect(
                user, target, stacks, sfSO, resist, RuntimeDuration, RuntimePotency);
            user.GiveStatus(heal);
        }
    }
}
