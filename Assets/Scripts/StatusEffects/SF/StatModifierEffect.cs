using UnityEngine;

public class StatModifierEffect : StatusEffectData
{
    private readonly StatsModifierSO modifier;
    public StatsModifierSO Modifier => modifier;

    public override string GetRuntimeDescription()
    {
        if (modifier == null || modifier.value == 0) return "Altera os atributos do personagem enquanto estiver ativo.";
        string operation = modifier.value > 0 ? "+" : string.Empty;
        string unit = modifier.flat ? string.Empty : "%";
        return $"{modifier.statType}: {operation}{modifier.value}{unit}.";
    }

    public StatModifierEffect(BattleChar source, BattleChar owner, int stacks, StatusEffectSO so,
        StatsModifierSO modifier, bool resist, int duration = -1)
        : base(source, owner, 1, so, resist, duration >= 0 ? duration : stacks, 1)
    {
        this.modifier = modifier;
    }

    public override void OnApply()
    {
        if (owner != null && modifier != null)
            owner.ReceiveStatsModifier(modifier);
    }

    public override void OnRoundEnd()
    {
        TickDuration();
    }

    public override void OnRemove()
    {
        if (owner != null && modifier != null)
            owner.RemoveStatsModifier(modifier);
    }
}