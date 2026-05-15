using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class BattleController : MonoBehaviour
{
    [SerializeField] Transform[] alliesSlots;
    [SerializeField] Transform[] enemiesSlots;
    [SerializeField] GameObject enemyPref, playerPref;
    [SerializeField] BattleData data;

    [SerializeField] List<BattleCharacter> Allies = new();
    [SerializeField] List<BattleCharacter> Enemies = new();
    [SerializeField] List<BattleCharacter> TurnOrder = new();
    public int currentRound = 1;
    int currentTurnIndex = 0;

    public void BattleStart(BattleData data)
    {
        SpawnEnemy(data.EnemiesData);
        GatherAllies(GameMaster.Instance.partyManager.Party);
        BuildTurnOrder();
        StartCoroutine(BattleLoop());
    }

    public void SpawnEnemy(List<EnemyStats> enemies)
    {
        Debug.Log("Tentou Spawnar");
        EnemyStats[] enemiesstats = enemies.ToArray();
        for(int i = 0; i < enemiesstats.Length; i++) 
        {
            GameObject enemy = Instantiate(enemyPref, enemiesSlots[i].position, Quaternion.identity);
            EnemyController stats = enemy.GetComponent<EnemyController>();
            stats.GetNewStats(enemiesstats[i]);
            stats.InitializeStats();
            Enemies.Add(stats);
        }
    }

    public void GatherAllies(List<PJStats> AlliesStats)
    {
        Debug.Log("Tentou Spawnar");
        PJStats[] alliesStats = AlliesStats.ToArray();
        for (int i = 0; i < alliesStats.Length; i++)
        {
            GameObject allie = Instantiate(playerPref, alliesSlots[i].position, Quaternion.identity);
            PJController stats = allie.GetComponent<PJController>();
            stats.GetNewStats(alliesStats[i]);
            stats.InitializeStats();
            Allies.Add(stats);
        }
    }

    public void BuildTurnOrder()
    {
        TurnOrder.Clear();
        TurnOrder.AddRange(Allies);
        TurnOrder.AddRange(Enemies);
        SetContext();
        TurnOrder = TurnOrder.OrderByDescending(c => c.Dex).ToList();
    }

    public void SetContext()
    {
        foreach (EnemyController n in Enemies)
        {
            n.ctx = new BattleContext(n, Enemies, Allies);
        }

        foreach(PJController pj in Allies)
        {
            pj.ctx = new BattleContext(pj, Allies, Enemies);
        }
    }

    public IEnumerator BattleLoop()
    {
        while (true)
        {
            BattleCharacter current = TurnOrder[currentTurnIndex];

            yield return StartCoroutine(current.ExecuteTurn());
            NextTurn();
        }
    }

    public void NextTurn()
    {
        if (currentTurnIndex >= TurnOrder.Count - 1) { currentTurnIndex = 0; currentRound++; return; }
        currentTurnIndex++;
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            BattleStart(data);
        }
    }
}
