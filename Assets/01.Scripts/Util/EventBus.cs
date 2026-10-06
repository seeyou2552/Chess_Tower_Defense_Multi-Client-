using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventBus
{
    private static readonly Dictionary<Type, List<Delegate>> _subscribers = new Dictionary<Type, List<Delegate>>();

    public static void Subscribe<T>(Action<T> callback)
    {
        if (callback == null)
            return;

        var eventType = typeof(T);
        if (!_subscribers.TryGetValue(eventType, out var eventSubscribers))
        {
            eventSubscribers = new List<Delegate>();
            _subscribers[eventType] = eventSubscribers;
        }

        if (!eventSubscribers.Contains(callback))
            eventSubscribers.Add(callback);
    }

    public static void Unsubscribe<T>(Action<T> callback)
    {
        if (callback == null)
            return;

        var eventType = typeof(T);
        if (_subscribers.TryGetValue(eventType, out var eventSubscribers))
        {
            eventSubscribers.Remove(callback);
            if (eventSubscribers.Count == 0)
                _subscribers.Remove(eventType);
        }
    }

    public static void Publish<T>(T payload)
    {
        var eventType = typeof(T);
        if (!_subscribers.TryGetValue(eventType, out var eventSubscribers))
            return;

        foreach (var subscriber in eventSubscribers.ToArray())
        {
            if (subscriber is Action<T> callback)
                callback(payload);
        }
    }
}