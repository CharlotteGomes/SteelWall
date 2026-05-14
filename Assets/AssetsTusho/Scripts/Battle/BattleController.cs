using UnityEngine;
using System.Collections.Generic;
public class BattleController : MonoBehaviour
{

    public void BattleStart(BattleData data)
    {
        SpawnEnemy(data.EnemiesData);
        //GatherAllies(PartyManager.Instance.Party);
        BeginTurnLoop();
    }

    public void SpawnEnemy(List<EnemyStats> enemies)
    {

    }

    public void GatherAllies(List<PJController> Allies)
    {

    }
    
    public void BeginTurnLoop()
    {

    }

    private void Awake()
    {
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
