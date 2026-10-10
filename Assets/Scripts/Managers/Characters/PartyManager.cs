using UnityEngine;
using System.Collections.Generic;
using System;
public class PartyManager : MonoBehaviour
{
    public CharacterGroupSO party;
    public static Action<PJData> OnPartyAdd;
    public static Action<PJData> OnPartyRemove;

    private void Awake()
    {
       
    }

    void Start()
    {
        
    }

    
    void Update()
    {
    }

    public void PartyAdd(PJData pj)
    {
        if (!party.AddParty(pj)) return;
        OnPartyAdd?.Invoke(pj);
    }
    public void PartyRemove(PJData pj)
    {
        if (!party.RemoveParty(pj)) return;
        OnPartyRemove?.Invoke(pj);
    }
}
