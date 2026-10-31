using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MarkCritModifier", menuName = "Scriptable Objects/SkillSO/Modifiers/MarkCritModifier")]
public class MarkCritModifier : SkillModifiersSO
{
    public int crit;
    public override void Prepare(SkillData data)
    {
        bool hasMarkedTarget = data.ctx != null && data.ctx.Target != null &&
            data.ctx.Target.Any(target => target != null && target.status_effects.Any(status => status is MarkEffect));
        if (hasMarkedTarget) data.crit_mod += crit;
    }
}