using System;
using System.Collections;
using UnityEngine;

public class GM : MonoBehaviour
{
    public static GM instance { get; private set; } //Singleton
    [SerializeField] Action game_state; //Current State

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
        game_state?.Invoke(); //game_state exists? Play Event
    }

    public void RunCoroutine(IEnumerator coroutine) //Static classes cannot run coroutines, so I created this method
    {
        if (coroutine != null)
        {
            StartCoroutine(coroutine);
        }
    }
}
