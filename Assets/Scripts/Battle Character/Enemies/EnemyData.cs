using UnityEngine;

public class EnemyData : BattleCharData 
{
    public EnemyBehaviourSO behaviour { get; private set; }
    public EnemyData(EnemySO so) : base(so)
    {
        behaviour = so.behaviour;
    }
}
