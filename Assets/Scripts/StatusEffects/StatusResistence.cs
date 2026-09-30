using UnityEngine;

public enum ResistenceType
{
    STUN,
    DEBUFF,
    BLEED,
    POISON,
    MOVE,
    SANITY
}

[System.Serializable]
public class StatusResistence
{
    public ResistenceType resist;
    public int value;

    public StatusResistence(ResistenceType resist, int value)
    {
        this.resist = resist;
        this.value = value;
    }
}
