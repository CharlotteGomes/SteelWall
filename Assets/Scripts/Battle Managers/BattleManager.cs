using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using NUnit.Framework;
using System.Collections;
public enum TurnStep
{
    ChooseSkill,
    ChooseTarget,
    ExecutingSkill,
    Finish
}

public class BattleManager : MonoBehaviour
{
    public BattlePresentation battle_presentation { get; private set; }

    [SerializeField] List<BattleChar> initiative = new List<BattleChar>();

    [SerializeField] List<Transform> pj_spawn, enemy_spawn = new List<Transform>();
    [SerializeField] GameObject pj_prefab, enemy_prefab;
    [SerializeField] List<PJSO> test1 = new List<PJSO>();
    [SerializeField] List<EnemySO> test2 = new List<EnemySO>();

    public BattleChar current { get; private set; }

    public int turn { get; private set; } = 1;
    [SerializeField] int initiative_index = 0;
    private void Awake()
    {
        battle_presentation = GetComponent<BattlePresentation>();
        StartBattle(test1, test2);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void StartBattle(List<PJSO> pjs, List<EnemySO> enemies)
    {
        SpawnCharacters(pjs, enemies);
        initiative_index = 0;
        StartCoroutine(TurnOrder());
    }
    
    IEnumerator TurnOrder()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            current = initiative[initiative_index];
            if (!current.CheckAlive()) { StartCoroutine(CharacterDeath(current)); current = null; continue; }
            yield return StartCoroutine(current.OnGoingTurn());
            initiative_index++;
            if (initiative_index > initiative.Count - 1) { initiative_index = 0; turn++; }
        }
    }

    void SpawnCharacters(List<PJSO> pjs, List<EnemySO> enemies)
    {
        List<BattleChar> PJS = new List<BattleChar>();  
        List<BattleChar> ENEMIES = new List<BattleChar>();
        for (int i = 0; i < pjs.Count; i++)
        {
            GameObject pj = Instantiate(pj_prefab, pj_spawn[i]);
            SpriteRenderer pj_rend = pj.GetComponent<SpriteRenderer>();
            pj_rend.sortingOrder = -1 * i;
            pj.name = pjs[i].Name;
            PjController ctrl = pj.GetComponent<PjController>();
            ctrl.SetBattleChar(pjs[i]);
            ctrl.InitStats();
            initiative.Add(ctrl);
            PJS.Add(ctrl);
        }

        for(int i = 0; i < enemies.Count; i++)
        {
            GameObject n = Instantiate(enemy_prefab, enemy_spawn[i]);
            SpriteRenderer spr = n.GetComponent<SpriteRenderer>();
            spr.flipX = true;
            spr.sortingOrder = -1 * i;
            n.name = enemies[i].Name;
            EnemyController ctrl = n.GetComponent<EnemyController>();
            ctrl.SetBattleChar(enemies[i]);
            ctrl.InitStats();
            initiative.Add(ctrl);
            ENEMIES.Add(ctrl);
        }
        SetInitiative();
        SetContext(PJS, ENEMIES);
    }

    void SetInitiative()
    {
        initiative = initiative.OrderByDescending(a => a.dexterity).ToList();   
    }

    void SetContext(List<BattleChar> pjs, List<BattleChar> enemies)
    {
        List<BattleChar> PJS = new List<BattleChar>();
        List<BattleChar> ENEMIES = new List<BattleChar>();

        foreach(BattleChar pj in pjs)
        {
            PJS.Add(pj);
        }
        foreach(BattleChar enemy in enemies)
        {
            ENEMIES.Add(enemy);
        }

        foreach(BattleChar b in initiative)
        {
            switch (b)
            {
                case PjController pj:
                    pj.CreateContext(PJS, ENEMIES);
                    break;
                case EnemyController enemy:
                    enemy.CreateContext(ENEMIES, PJS);
                    break;
            }
        }
    }
    void InitiativeRemove(BattleChar b)
    {
        if (initiative.Contains(b)) { initiative.Remove(b); }
    }

    void UpdateContext(BattleContext ctx)
    {
        foreach(BattleChar b in ctx.Enemies)
        {
            if (!initiative.Contains(b)) { ctx.Enemies.Remove(b); }
            
        }

        foreach(BattleChar b in ctx.Party)
        {
            if (!initiative.Contains(b)) { ctx.Party.Remove(b); }
            
        }
    }

    IEnumerator CharacterDeath(BattleChar b)
    {
        b.OnCharacterDeath();
        InitiativeRemove(b);
        foreach(BattleChar c in initiative)
        {
            UpdateContext(c.Context);
        }
        yield return null;
    }

    void UpdatePos()
    {
    }
}
