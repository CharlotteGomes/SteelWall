using UnityEngine;

[CreateAssetMenu(fileName = "DamageEffect", menuName = "Scriptable Objects/SkillSO/Effects/Damage")]
public class DamageTarget : SkillEffectSO
{
    [Range(0, 10)]
    public float dmg;

    public override void Execute(BattleChar user, BattleResult result)
    {
        if (user == null || result == null) return;

        if (self)
        {
            int selfDamage = RollDamage(user, false);
            user.TakeDamage(new DamageInfo(user, user, selfDamage, true));
            return;
        }

        // Each target receives an independent damage roll and uses its own critical result.
        foreach (BattleChar target in result.hit)
        {
            if (target == null || !target.CheckAlive()) continue;
            TargetResolution resolution = result.GetTargetResult(target);
            int damage = RollDamage(user, resolution != null && resolution.critical);
            user.DealDamage(new DamageInfo(user, target, damage));
        }
    }

    private int RollDamage(BattleChar user, bool critical)
    {
        int minDamage = Mathf.Max(0, (int)(user.GetStat(StatType.DMG_MIN) * dmg));
        int maxDamage = Mathf.Max(minDamage, (int)(user.GetStat(StatType.DMG_MAX) * dmg));
        if (critical)
        {
            minDamage *= 2;
            maxDamage *= 2;
        }
        return Random.Range(minDamage, maxDamage + 1);
    }
}
