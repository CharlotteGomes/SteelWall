using System.Xml.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[CreateAssetMenu(fileName = "Move", menuName = "Scriptable Objects/SkillSO/Effects/Status Effect/HealPerTurn")]
public class HealPerTurnEffectSO : ApplyStatusEffect
{
    public override void Execute(BattleChar user, BattleResult result)
    {
        if (self)
        {
            HealPerTurnEffect heal = new HealPerTurnEffect(user, user, sfSO, name);
            user.GiveStatus(heal);
            return;
        }

        foreach (BattleChar target in result.hit)
        {

            HealPerTurnEffect heal = new HealPerTurnEffect(user, target, sfSO, name);
            user.GiveStatus(heal);
        }
    }
}
