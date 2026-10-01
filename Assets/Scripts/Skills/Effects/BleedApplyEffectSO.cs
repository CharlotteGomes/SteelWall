using UnityEngine;

[CreateAssetMenu(fileName = "Bleed", menuName = "Scriptable Objects/SkillSO/Effects/Status Effect/BleedEffect")]
public class BleedApplyEffectSO : ApplyStatusEffect
{
    public override void Execute(BattleChar user, BattleResult result)
    {
        if (self)
        {
            BleedEffect bleed = new BleedEffect(user, user, sfSO, name);
            user.GiveStatus(bleed);
            return;
        }

        foreach (BattleChar target in result.hit)
        {
            BleedEffect bleed = new BleedEffect(user, target, sfSO, name);
            user.GiveStatus(bleed);
        }
    }
}
