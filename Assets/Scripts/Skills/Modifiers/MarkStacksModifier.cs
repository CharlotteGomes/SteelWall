using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MarkStacksModifier", menuName = "Scriptable Objects/SkillSO/Modifiers/MarkStacksModifier")]
public class MarkStacksModifier : SkillModifiersSO
{
    public int stacks;
    public override void Prepare(SkillData data)
    {
        bool hasMarkedTarget = data.ctx != null && data.ctx.Target != null &&
            data.ctx.Target.Any(target => target != null && target.status_effects.Any(status => status is MarkEffect));
        if (!hasMarkedTarget) return;
        foreach (SkillEffectSO effect in data.effects)
            if (effect is ApplyStatusEffect statusEffect) statusEffect.stacks += stacks;
    }
}