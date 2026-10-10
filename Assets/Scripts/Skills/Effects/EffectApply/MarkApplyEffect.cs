using UnityEngine;

[CreateAssetMenu(fileName = "MarkApplyEffect", menuName = "Scriptable Objects/SkillSO/Effects/Status Effect/MarkApplyEffect")]
public class MarkApplyEffect : ApplyStatusEffect
{
    public override void Execute(BattleChar user, BattleResult result)
    {
        if (self)
        {
            MarkEffect bleed = new MarkEffect(user, user, sfSO, stacks, resist);
            user.GiveStatus(bleed);
            return;
        }

        foreach (BattleChar target in result.hit)
        {
            MarkEffect bleed = new MarkEffect(user, target, sfSO, stacks, resist);
            user.GiveStatus(bleed);
        }
    }
}
