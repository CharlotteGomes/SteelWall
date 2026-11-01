using UnityEngine;

public class DamageModifierEffect : StatusEffectData
{
    private readonly int value;
    public int Value => value;

    public override string GetRuntimeDescription()
    {
        if (value == 0) return "Modifica o dano causado pelo personagem.";
        string operation = value > 0 ? "+" : string.Empty;
        return $"Modifica o dano causado em {operation}{value}%.";
    }

    public DamageModifierEffect(BattleChar source, BattleChar owner, StatusEffectSO so, int stacks,
        int value, bool resist, int duration = -1)
        : base(source, owner, 1, so, resist, duration >= 0 ? duration : stacks, 1)
    {
        this.value = value;
    }

    public override void OnDamageDealt(DamageInfo info)
    {
        if (info == null) return;
        info.damage += (info.damage * value) / 100;
        TickDuration();
    }
}