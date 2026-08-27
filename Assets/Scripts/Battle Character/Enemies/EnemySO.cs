using UnityEngine;

[CreateAssetMenu(fileName = "BattleCharSO", menuName = "Scriptable Objects/Enemy/EnemyBase")]
public class EnemySO : BattleCharSO
{
    public int life_max;
    public int r_slashing, r_bludgeoning, r_pierce, r_fire, r_ice, r_lightning, r_purity;
    public EnemyBehaviourSO behaviour;
}
