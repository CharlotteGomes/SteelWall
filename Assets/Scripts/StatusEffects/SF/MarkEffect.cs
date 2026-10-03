using UnityEngine;

public class MarkEffect : StatusEffectData
{
    public MarkEffect(BattleChar source, BattleChar owner, StatusEffectSO so, int stacks, bool resist) : base(source, owner, stacks, so, resist)
    {
    }
}
