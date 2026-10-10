using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GM : MonoBehaviour
{
    public static GM instance { get; private set; } //Singleton
    public PartyManager party_manager;
    public BattleManager battle_manager;

    private void Awake()
    {
        if(instance != this && instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); //Permanent Obj
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space)) { SceneManager.LoadScene("CombatScene"); }
    }
}
