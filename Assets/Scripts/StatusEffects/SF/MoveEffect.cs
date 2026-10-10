using UnityEngine;

[System.Serializable]
public class MoveEffect : StatusEffectData
{
    [SerializeField] int direction;
    public MoveEffect(BattleChar source, BattleChar owner, int stacks, StatusEffectSO so, int direction, bool resist) : base(source, owner, stacks, so, resist)
    {
        this.direction = direction;
    }

    public override void OnApply()
    {
        owner.ConfirmSwapTarget(owner.GetSelfPos() + direction);
    }
}
