using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
public class BattleManager : MonoBehaviour
{
    //public static Action<List<PJSO>, List<EnemySO>> start_battle;

    [SerializeField] List<BattleChar> initiative = new List<BattleChar>();
    [SerializeField] List<PjController> pj_on_battle =  new List<PjController>();
    [SerializeField] List<EnemyController> enemy_on_battle = new List<EnemyController>();

    [SerializeField] List<Transform> pj_spawn, enemy_spawn = new List<Transform>();
    [SerializeField] GameObject pj_prefab, enemy_prefab;
    [SerializeField] List<PJSO> test1 = new List<PJSO>();
    [SerializeField] List<EnemySO> test2 = new List<EnemySO>();

    private void Awake()
    {
        //start_battle += SpawnCharacters;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L)) { SpawnCharacters(test1, test2);}
    }

    void SpawnCharacters(List<PJSO> pjs, List<EnemySO> enemies)
    {
        for(int i = 0; i < pjs.Count; i++)
        {
            GameObject pj = Instantiate(pj_prefab, pj_spawn[i]);
            PjController ctrl = pj.GetComponent<PjController>();
            ctrl.SetBattleChar(pjs[i]);
            ctrl.InitStats();
            pj_on_battle.Add(ctrl);
        }

        for(int i = 0; i < enemies.Count; i++)
        {
            GameObject n = Instantiate(enemy_prefab, enemy_spawn[i]);
            EnemyController ctrl = n.GetComponent<EnemyController>();
            ctrl.SetBattleChar(enemies[i]);
            ctrl.InitStats();
            enemy_on_battle.Add(ctrl);
        }
        SetInitiative();
        SetContext();
    }

    void SetInitiative()
    {
        List<BattleChar> battle = new List<BattleChar>();
        foreach(EnemyController enemy in enemy_on_battle)
        {
            battle.Add(enemy);
        }
        foreach(PjController pj in pj_on_battle)
        {
            battle.Add(pj);
        }
        initiative = battle.OrderByDescending(a => a.ReadStats(Stats.Dexterity)).ToList();   
    }

    void SetContext()
    {
        foreach(BattleChar c in initiative)
        {
            if (pj_on_battle.Contains(c))
            {
                c.CreateContext(pj_on_battle.Cast<BattleChar>().ToList(), enemy_on_battle.Cast<BattleChar>().ToList());
            }
        }
    }
    void InitiativeRemove(BattleChar b)
    {
        if (initiative.Contains(b)) { initiative.Remove(b); }
    }
}
