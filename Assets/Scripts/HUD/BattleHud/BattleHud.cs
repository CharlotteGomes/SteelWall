using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;
public class BattleHud : MonoBehaviour
{
    public PJBattleHUD pj_hud;
    public EnemyBattleHUD enemy_hud;
    public LifeBarManager lifeManager;
    public IndicatorManager indicatorManager;
    public void SetCharacters(BattleChar pj, BattleChar n) 
    {
        SetCharacter(pj);
        SetCharacter(n);
        
    }

    public void SetCharacter(BattleChar c)
    {
        if (c is PjController pj) { pj_hud.SetCharacter(c); return; }
        if (c is EnemyController enemy) {enemy_hud.SetCharacter(c); }
    }

    public void RefreshHUD()
    {
        pj_hud.UpdateChildren();
        enemy_hud.UpdateChildren();
        lifeManager.UpdateLifeBars();
    }

    public void OnStartTurn(BattleChar c)
    {
        SetCharacter(c);
        SubscribeEvents(c);
    }

    public void SubscribeEvents(BattleChar c)
    {
        if(c is PjController pj) { pj_hud.SubscriveEvents(pj); }
        if(c is EnemyController n) { enemy_hud.SubscriveEvents(n); }
        indicatorManager.SubscribeEvents(c);
    }
    public void UnsubscribeEvents(BattleChar c)
    {
        if (c is PjController pj) { pj_hud.UnsubscriveEvents(pj); }
        if (c is EnemyController n) { enemy_hud.UnsubscriveEvents(n); }
        indicatorManager.UnsubscribeEvents(c);
    }
}
