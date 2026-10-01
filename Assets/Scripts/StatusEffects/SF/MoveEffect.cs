using UnityEngine;

[System.Serializable]
public class MoveEffect : StatusEffectData
{
    [SerializeField] int direction;
    public MoveEffect(BattleChar source, BattleChar owner, StatusEffectSO so, int direction, string efName) : base(source, owner, so, efName)
    {
        this.direction = direction;
    }

    public override void OnApply()
    {
        owner.ConfirmSwapTarget(owner.GetSelfPos() + direction);
    }
}
