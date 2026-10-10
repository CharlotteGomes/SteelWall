using UnityEngine;

[CreateAssetMenu(fileName = "MarkCritModifier", menuName = "Scriptable Objects/SkillSO/Modifiers/MarkCritModifier")]
public class MarkCritModifier : SkillModifiersSO
{
    public int crit;
    public override void Prepare(SkillData data)
    {
        foreach (StatusEffectData f in data.ctx.Target[0].status_effects)
        {
            if (f is not MarkEffect) continue;
            data.crit_mod += crit;
        }
    }
}