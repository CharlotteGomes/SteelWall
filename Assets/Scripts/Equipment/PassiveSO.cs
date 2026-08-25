using System;
using UnityEditor.Rendering;
using UnityEngine;

public enum BattleEvent 
{
    StartTurn,
    EndTurn,
    DealDamage,
    TakeDamage,
    DealHeal,
    ReceiveHeal,
    ApplyStatusEffect,
    ReceiveStatusEffect
}

public abstract class PassiveSO : ScriptableObject
{
    public string Name, description;
    public BattleEvent battle_event;

    public abstract void Passive(BattleChar character);
    public void Apply(BattleChar character)
    {
        switch (battle_event)
        {
            case BattleEvent.StartTurn:
                character.start_turn += Passive; break;
            case BattleEvent.EndTurn:
                character.end_turn += Passive; break;
            case BattleEvent.DealDamage:
                character.deal_damage += Passive; break;
            case BattleEvent.TakeDamage:
                character.take_damage += Passive; break;
            case BattleEvent.DealHeal:
                character.deal_heal += Passive; break;
            case BattleEvent.ReceiveHeal:
                character.receive_heal += Passive; break;
            case BattleEvent.ApplyStatusEffect:
                character.apply_status_effect += Passive; break;
            case BattleEvent.ReceiveStatusEffect:
                character.receive_status_effect += Passive; break;
        }
    }
    public void Remove(BattleChar character)
    {
        switch (battle_event)
        {
            case BattleEvent.StartTurn:
                character.start_turn -= Passive; break;
            case BattleEvent.EndTurn:
                character.end_turn -= Passive; break;
            case BattleEvent.DealDamage:
                character.deal_damage -= Passive; break;
            case BattleEvent.TakeDamage:
                character.take_damage -= Passive; break;
            case BattleEvent.DealHeal:
                character.deal_heal -= Passive; break;
            case BattleEvent.ReceiveHeal:
                character.receive_heal -= Passive; break;
            case BattleEvent.ApplyStatusEffect:
                character.apply_status_effect -= Passive; break;
            case BattleEvent.ReceiveStatusEffect:
                character.receive_status_effect -= Passive; break;
        }
    }
}
