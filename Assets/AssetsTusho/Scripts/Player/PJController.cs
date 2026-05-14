using UnityEngine;
using System;
public class PJController : Character, IDamageable, IKillable, IBattleUnit
{
    public Action<PJController> OnDeath;

    #region interfaces
    public void OnTurnStart()
    {
        Debug.Log($"Começou seu turno {Name}");
    }

    public void OnTurnEnd()
    {
        Debug.Log($"Seu turno acabou {Name}");
    }

    public void TakeDamage(int damage)
    {
        if (damage >= 0) { LifeCurrent -= damage; }
        if(LifeCurrent < 0) {}
    }

    public void Die()
    {
        LifeCurrent = 0;
        OnDeath?.Invoke(this);
    }
    #endregion
    void Start()
    {
        InitializeStats();
    }
    
    void Update()
    {
        
    }
}
