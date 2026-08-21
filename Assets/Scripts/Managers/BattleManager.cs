using UnityEngine;
using System.Collections.Generic;
using System;
public class BattleManager : MonoBehaviour
{
    [SerializeField] BattleChar[] initiative;
    [SerializeField] PjController[] pj_on_battle;
    [SerializeField] EnemyController[] enemy_on_battle;
    [SerializeField] Transform[] pj_spawn, enemy_spawn;
    [SerializeField] GameObject pj_prefab, enemy_prefab;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PjSpawn()
    {
        PjController[] pjs = new PjController[4];
        GM.instance.party_manager.party_list_read.CopyTo(pjs, 0);
        for(int i = 0; i < pjs.Length; i++)
        {
            GameObject pj = Instantiate(pj_prefab, pj_spawn[i]);
            PjController ctrl = pj.GetComponent<PjController>();
        }
    }
}
