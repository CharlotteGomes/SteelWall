using UnityEngine;

[CreateAssetMenu(fileName = "HealEffect", menuName = "Scriptable Objects/SkillSO/Effects/Heal")]
public class HealEffectSO : SkillEffectSO
{
    public int max_heal;
    public int min_heal;
    public bool unique_value;

    public override void Execute(BattleChar user, BattleResult result)
    {
        foreach (BattleChar target in result.hit)
        {
            int heal_amount = unique_value ? max_heal : Random.Range(min_heal, max_heal + 1);
            HealInfo info = new HealInfo(user, heal_amount, target);
            user.DealHeal(info);
        }
    }
}
