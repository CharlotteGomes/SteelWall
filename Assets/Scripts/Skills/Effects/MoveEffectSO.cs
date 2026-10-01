using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Scriptable Objects/SkillSO/Effects/Status Effect/Move")]
public class MoveEffectSO : ApplyStatusEffect
{
    public int dir = 0;
    public override void Execute(BattleChar user, BattleResult result)
    {
        if (self)
        {
            MoveEffect move = new MoveEffect(user, user, sfSO, dir, name);
            user.GiveStatus(move);
            return;
        }

        foreach (BattleChar target in result.hit)
        {
            MoveEffect move = new MoveEffect(user, target, sfSO, dir, name);
            user.GiveStatus(move);
        }
    }
}
