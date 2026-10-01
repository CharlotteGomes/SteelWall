using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Search;
public class BattleHud : MonoBehaviour
{
    public PJBattleHUD pj_hud;
    public EnemyBattleHUD enemy_hud;
    public void SetCharacter(BattleChar pj, BattleChar n) 
    {
        SetCharacterPJ(pj);
        SetCharacterEnemy(n);
    }

    public void SetCharacterPJ(BattleChar c)
    { 
        pj_hud.SetCharacter(c);
        pj_hud.UpdateChildren();
    }
    public void SetCharacterEnemy(BattleChar c)
    {
        enemy_hud.SetCharacter(c);
        enemy_hud.UpdateChildren();
    }

    public void RefreshHUD()
    {
        pj_hud.UpdateChildren();
        enemy_hud.UpdateChildren();
    }
}
