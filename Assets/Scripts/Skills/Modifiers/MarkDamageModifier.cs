using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MarkDamageModifier", menuName = "Scriptable Objects/SkillSO/Modifiers/MarkDamageModifier")]
public class MarkDamageModifier : SkillModifiersSO
{
    [Range(0, 10)]
    public float damage;
    public override void Prepare(SkillData data)
    {
        bool hasMarkedTarget = data.ctx != null && data.ctx.Target != null &&
            data.ctx.Target.Any(target => target != null && target.status_effects.Any(status => status is MarkEffect));
        if (!hasMarkedTarget) return;
        foreach (SkillEffectSO effect in data.effects)
            if (effect is DamageTarget damageEffect) damageEffect.dmg += damage;
    }
}