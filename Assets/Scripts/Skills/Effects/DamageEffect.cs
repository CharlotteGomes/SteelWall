using UnityEngine;

[CreateAssetMenu(fileName = "DamageEffect", menuName = "Scriptable Objects/SkillSO/Effects/Damage")]
public class DamageTarget: SkillEffectSO
{
    [Range(0, 2)]
    public float dmg;
    public override void Execute(BattleChar user, BattleResult result)
    {
        int min_damage = result.crit == true ? (int)(user.GetStat(StatType.DMG_MIN) * dmg) * 2 : (int)(user.GetStat(StatType.DMG_MIN) * dmg);
        int max_damage = result.crit == true ? (int)(user.GetStat(StatType.DMG_MAX) * dmg) * 2 : (int)(user.GetStat(StatType.DMG_MAX) * dmg);
        Debug.Log($"{min_damage} | {max_damage}");
        int damage = Random.Range(min_damage, max_damage + 1);

        if (self)
        {
            user.TakeDamage(new DamageInfo(user, user, damage, true));
            return;
        }

        foreach (BattleChar target in result.hit)
        {
            DamageInfo info = new DamageInfo(user, target, damage);
            user.DealDamage(info);
        }
    }
}
