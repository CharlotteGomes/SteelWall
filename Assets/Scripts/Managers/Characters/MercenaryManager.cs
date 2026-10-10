using System.Collections.Generic;
using UnityEngine;

public class MercenaryManager : MonoBehaviour
{
    public CharacterGroupSO hired;
    public List<PJSO> test;

    public void Awake()
    {

    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            foreach (PJSO pj in test)
            {
                hired.AddParty(new PJData(pj));
            }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log(hired.party.Count);
            GM.instance.party_manager.PartyAdd(hired.party[0]);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
            GM.instance.party_manager.PartyRemove(GM.instance.party_manager.party.party[0]);
    }

    void HireChar(PJData pj)
    {
        if (!hired.AddParty(pj)) return;
    }
    void RemoveChar(PJData pj)
    {
        if (!hired.RemoveParty(pj)) return;
    }

    public void SubscribePartyEvents()
    {
        PartyManager.OnPartyAdd += RemoveChar;
        PartyManager.OnPartyRemove += HireChar;
    }

    public void UnsubscribePartyEvents() 
    {
        PartyManager.OnPartyAdd += RemoveChar;
        PartyManager.OnPartyRemove += HireChar;
    }
}
