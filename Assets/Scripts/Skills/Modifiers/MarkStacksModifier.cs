using UnityEngine;

[CreateAssetMenu(fileName = "MarkStacksModifier", menuName = "Scriptable Objects/SkillSO/Modifiers/MarkStacksModifier")]
public class MarkStacksModifier : SkillModifiersSO
{
    public int stacks;
    public override void Prepare(SkillData data)
    {
        foreach (StatusEffectData f in data.ctx.Target[0].status_effects)
        {
            if (f is not MarkEffect) continue;
            foreach (var effects in data.effects)
            {
                if (effects is not ApplyStatusEffect a) continue;
                a.stacks += stacks;
            }
        }
    }
}
