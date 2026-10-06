using UnityEngine;
using System.Collections.Generic;
public class StatsPanel : MonoBehaviour
{
    [SerializeField] protected BattleChar character;
    [SerializeField] protected List<StatsToText> statsToText = new List<StatsToText>();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCharacter(BattleChar character)
    {
        this.character = character;
        UpdateChildren();
    }

    public virtual void UpdateChildren()
    {
        foreach (StatsToText t in statsToText)
        {
            if (!t.isActiveAndEnabled) continue;
            t.UpdateCharacter(character);
            t.UpdateText();
        }
    }

    public virtual void SubscriveEvents(BattleChar character)
    {

    }

    public virtual void UnsubscriveEvents(BattleChar character)
    {

    }

    protected void OnEnable()
    {
        if(character != null)
        {
            SubscriveEvents(character);
        }
    }

    protected void OnDisable()
    {
        if(character != null)
        {
            UnsubscriveEvents(character);
        }
    }
}
