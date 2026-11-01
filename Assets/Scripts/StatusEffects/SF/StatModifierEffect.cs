using UnityEngine;

public class StatModifierEffect : StatusEffectData
{
    private readonly StatsModifierSO sourceModifier;
    private readonly StatsModifierSO runtimeModifier;
    private bool runtimeModifierReleased;

    public StatsModifierSO Modifier => runtimeModifier;

    public override string GetRuntimeDescription()
    {
        if (runtimeModifier == null || runtimeModifier.value == 0)
            return "Altera os atributos do personagem enquanto estiver ativo.";

        string operation = runtimeModifier.value > 0 ? "+" : string.Empty;
        string unit = runtimeModifier.flat ? string.Empty : "%";
        return $"{runtimeModifier.statType}: {operation}{runtimeModifier.value}{unit}.";
    }

    public StatModifierEffect(BattleChar source, BattleChar owner, int stacks, StatusEffectSO so,
        StatsModifierSO modifier, bool resist, int duration = -1)
        : base(source, owner, Mathf.Max(1, stacks), so, resist,
            duration >= 0 ? duration : stacks, Mathf.Max(1, stacks))
    {
        sourceModifier = modifier;
        runtimeModifier = modifier != null ? ScriptableObject.Instantiate(modifier) : null;
        if (runtimeModifier != null)
            runtimeModifier.value *= Mathf.Max(1, stacks);
    }

    public override void OnApply()
    {
        if (owner != null && runtimeModifier != null)
            owner.ReceiveStatsModifier(runtimeModifier);
    }

    public override void OnRoundEnd()
    {
        TickDuration();
    }

    public override void OnStackPile(StatusEffectData status)
    {
        if (status == null) return;
        int previousStacks = stacks;
        base.OnStackPile(status);
        int addedStacks = stacks - previousStacks;
        if (addedStacks > 0 && runtimeModifier != null && sourceModifier != null)
            runtimeModifier.value += sourceModifier.value * addedStacks;
    }

    public override void OnRemove()
    {
        if (owner != null && runtimeModifier != null)
            owner.RemoveStatsModifier(runtimeModifier);
        ReleaseRuntimeModifier();
    }

    public override void OnDiscarded()
    {
        ReleaseRuntimeModifier();
    }

    private void ReleaseRuntimeModifier()
    {
        if (runtimeModifierReleased || runtimeModifier == null) return;
        runtimeModifierReleased = true;
        Object.Destroy(runtimeModifier);
    }
}