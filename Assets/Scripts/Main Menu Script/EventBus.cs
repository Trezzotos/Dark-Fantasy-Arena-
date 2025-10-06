using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class EventBus {
    public static event Action<string, object> OnEvent;
    public static void Publish(string topic, object payload = null) => OnEvent?.Invoke(topic, payload);
    public static void Subscribe(Action<string, object> handler) => OnEvent += handler;
    public static void Unsubscribe(Action<string, object> handler) => OnEvent -= handler;
}
