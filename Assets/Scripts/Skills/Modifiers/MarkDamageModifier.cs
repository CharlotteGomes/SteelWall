using UnityEngine;

[CreateAssetMenu(fileName = "MarkDamageModifier", menuName = "Scriptable Objects/SkillSO/Modifiers/MarkDamageModifier")]
public class MarkDamageModifier : SkillModifiersSO
{
    [Range(0, 10)]
    public float damage;
    public override void Prepare(SkillData data)
    {
        foreach(StatusEffectData f in data.ctx.Target[0].status_effects)
        {
            if (f is not MarkEffect) continue;
            foreach (var effects in data.effects)
            {
                if (effects is not DamageTarget d) continue;
                d.dmg += damage;
            }
        }
    }
}
