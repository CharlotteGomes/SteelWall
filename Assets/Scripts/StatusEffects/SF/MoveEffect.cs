using UnityEngine;

[System.Serializable]
public class MoveEffect : StatusEffectData
{
    [SerializeField] int direction;

    public MoveEffect(BattleChar source, BattleChar owner, int stacks, StatusEffectSO so, int direction, bool resist)
        : base(source, owner, stacks, so, resist)
    {
        this.direction = direction;
    }

    public override void OnApply()
    {
        if (owner == null) return;
        owner.ConfirmSwapTarget(owner.GetSelfPos() + direction);
        // Movement is an immediate action, not a persistent status instance.
        owner.StatusRemove(this);
    }
}