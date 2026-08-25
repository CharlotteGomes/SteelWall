using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    private static readonly Dictionary<Type, Delegate> events = new();

    public static void Subscribe<T>(Action<T> callback)
    {
        if (events.TryGetValue(typeof(T), out var del))
            events[typeof(T)] = Delegate.Combine(del, callback);
        else
            events[typeof(T)] = callback;
    }

    public static void Unsubscribe<T>(Action<T> callback)
    {
        if (events.TryGetValue(typeof(T), out var del))
            events[typeof(T)] = Delegate.Remove(del, callback);
    }

    public static void Publish<T>(T message)
    {
        if (events.TryGetValue(typeof(T), out var del))
            ((Action<T>)del)?.Invoke(message);
    }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(Instance);
    }
    

}
