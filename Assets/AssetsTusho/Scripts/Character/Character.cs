using UnityEngine;
using UnityEngine.AdaptivePerformance;

public abstract class Character : MonoBehaviour
{
    [SerializeField] protected BaseStats stats;
    public string Name;

    [Header("Primary Stats")]
    public int LifeMax => 10 + (Vig * 3);
    public int ManaMax => 5 + (Int * 2);
    [SerializeField] protected int LifeCurrent;
    [SerializeField] protected int ManaCurrent;
    public int lifeCurrent => LifeCurrent;
    public int manaCurrent => ManaCurrent;

    [Header("Secondary Stats")]
    public int Str {  get; private set; }
    public int Dex { get; private set; }
    public int Vig {  get; private set; }
    public int Int { get; private set; }
    public int Kno { get; private set; }
    public int Fai { get; private set; }
        
    public virtual void InitializeStats()
    {
        Name = stats.Name;
        Str = stats.strength;
        Dex = stats.dexterity;
        Vig = stats.vigor;
        Int = stats.inteligence;
        Kno = stats.knowledge;
        Fai = stats.faith;
        LifeCurrent = LifeMax;
        ManaCurrent = ManaMax;
    }

    public virtual void GetNewStats(BaseStats stats)
    {
        this.stats = stats;
    }
}
