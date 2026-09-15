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
    public BattleHUD hud;
    [SerializeField] List<BattleChar> initiative = new List<BattleChar>();
    [SerializeField] GameObject pj_prefab, enemy_prefab;
    [SerializeField] List<PJSO> test1 = new List<PJSO>();
    [SerializeField] List<EnemySO> test2 = new List<EnemySO>();

    [SerializeField] BattleChar[] pj_slot = new BattleChar[4];
    [SerializeField] BattleChar[] enemy_slot = new BattleChar[4];


    public BattleChar current { get; private set; }

    public int turn { get; private set; } = 1;
    [SerializeField] int initiative_index = 0;
    private void Awake()
    {
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
            if(current is PjController pj){ hud.current = pj; hud.UpdateSkills(); } 
            yield return StartCoroutine(current.OnGoingTurn());
            initiative_index++;
            if (initiative_index > initiative.Count - 1) { initiative_index = 0; turn++; }
            Debug.Log("FIM DE TURNO");
        }
    }

    void SpawnCharacters(List<PJSO> pjs, List<EnemySO> enemies)
    {
        for (int i = 0; i < pjs.Count; i++)
        {
            GameObject pj = Instantiate(pj_prefab);
            PjController ctrl = pj.GetComponent<PjController>();
            pj_slot[i] = ctrl; 
            ctrl.sprite_renderer.sortingOrder = -1 * i;
            pj.name = pjs[i].Name;

            ctrl.SetBattleChar(pjs[i]);
            ctrl.InitStats();
            initiative.Add(ctrl);
        }

        for(int i = 0; i < enemies.Count; i++)
        {
            GameObject n = Instantiate(enemy_prefab);
            EnemyController ctrl = n.GetComponent<EnemyController>();
            enemy_slot[i] = ctrl;
            ctrl.sprite_renderer.flipX = true;
            ctrl.sprite_renderer.sortingOrder = -1 * i;
            n.name = enemies[i].Name;

            
            ctrl.SetBattleChar(enemies[i]);
            ctrl.InitStats();
            initiative.Add(ctrl);
        }
        SetInitiative();
        SetContext();
        SetAllPosition();
    }

    void SetInitiative()
    {
        initiative = initiative.OrderByDescending(a => a.GetStat(StatType.SPD)).ToList();   
    }

    void SetContext()
    {
        for(int i = 0; i < pj_slot.Length; i++)
        {
            if (pj_slot[i] != null)
            {
                pj_slot[i].CreateContext(pj_slot.Where(b => b != null).ToList(), enemy_slot.Where(b => b != null).ToList(), i);
            }
        }
        for (int i = 0; i < enemy_slot.Length; i++)
        {
            if (enemy_slot[i] != null)
            {
                enemy_slot[i].CreateContext(enemy_slot.Where(b => b != null).ToList(), pj_slot.Where(b => b != null).ToList(), i);
            }
        }
    }

    void SetAllPosition()
    {
        for(int i = 0; i < pj_slot.Length; i++)
        {
            if(pj_slot[i] != null) { pj_slot[i].transform.position = (Vector3)(i * 2.5f * Vector2.left) + new Vector3(-3, 1f, -5); }
        }
        for(int i = 0; i < enemy_slot.Length; i++)
        {
            if(enemy_slot[i] != null) { enemy_slot[i].transform.position = (Vector3)(i * 2.5f * Vector2.right) + new Vector3(3, 1f, -5); }
        }
    }

    public void InitiativeRemove(BattleChar b)
    {
        if (initiative.Contains(b)) { initiative.Remove(b); }
    }

    public void SlotRemove(BattleChar b)
    {
        switch(b)
        {
            case PjController pj:
                for(int i = 0; i < pj_slot.Length; i++)
                {
                    if(pj_slot[i] == pj) { pj_slot[i] = null; break; }
                }
                break;
            case EnemyController enemy:
                for(int i = 0; i < enemy_slot.Length; i++)
                {
                    if(enemy_slot[i] == enemy) { enemy_slot[i] = null; break; }
                }
                break;
        }
    }

    IEnumerator CharacterDeath(BattleChar b)
    {
        InitiativeRemove(b);
        SlotRemove(b);
        SetContext();
        yield return null;
    }

    void UpdatePos()
    {
    }
}
