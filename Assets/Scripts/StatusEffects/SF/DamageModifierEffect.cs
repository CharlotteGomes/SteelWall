using UnityEngine;

public class DamageModifierEffect : StatusEffectData
{
    private int value;
    public int Value => value;

    public override string GetRuntimeDescription()
    {
        if (value == 0) return "Modifica o dano causado pelo personagem.";
        string operation = value > 0 ? "+" : string.Empty;
        return $"Modifica o dano causado em {operation}{value}%.";
    }

    public DamageModifierEffect(BattleChar source, BattleChar owner, StatusEffectSO so, int stacks,
        int value, bool resist, int duration = -1)
        : base(source, owner, Mathf.Max(1, stacks), so, resist,
            duration >= 0 ? duration : stacks, Mathf.Max(1, stacks))
    {
        this.value = value * Mathf.Max(1, stacks);
    }

    public override void OnDamageDealt(DamageInfo info)
    {
        if (info == null || info.damage <= 0) return;
        info.damage += (info.damage * value) / 100;
    }

    public override void OnRoundEnd()
    {
        TickDuration();
    }

    public override void OnStackPile(StatusEffectData status)
    {
        if (status is not DamageModifierEffect incoming) return;
        base.OnStackPile(status);
        value += incoming.value;
    }
}