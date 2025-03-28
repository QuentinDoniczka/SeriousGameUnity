using System;
using System.Collections.Generic;
using UnityEngine;


namespace Project.Core.Events
{
    /// <summary>
    /// Event manager for communication between game components
    /// </summary>
    /// <remarks>
    /// Detailed file description:
    /// - Implements a typed event system based on the Singleton design pattern
    /// - Uses generic templates to allow the use of different enumeration types
    /// - Provides methods to subscribe, unsubscribe and trigger events
    /// - Events are stored in a dictionary with enum types as keys
    /// - The manager persists between scenes thanks to DontDestroyOnLoad
    /// </remarks>
    /// <author>Quentin Doniczka</author>
    /// <date>28/03/2025</date>
    public class EventManager : MonoBehaviour
    {
        private static EventManager _instance;
        public static EventManager Instance => _instance ??= CreateInstance();

        private readonly Dictionary<Enum, Delegate> _events = new();

        private static EventManager CreateInstance()
        {
            var go = new GameObject("EventManager");
            _instance = go.AddComponent<EventManager>();
            DontDestroyOnLoad(go);
            return _instance;
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Subscribe<T>(T eventType, Action handler) where T : Enum
        {
            if (!_events.ContainsKey(eventType))
            {
                _events[eventType] = handler;
                return;
            }
            _events[eventType] = Delegate.Combine(_events[eventType], handler);
        }

        public void Unsubscribe<T>(T eventType, Action handler) where T : Enum
        {
            if (!_events.ContainsKey(eventType)) return;
            _events[eventType] = Delegate.Remove(_events[eventType], handler);
        }

        public void TriggerEvent<T>(T eventType) where T : Enum
        {
            if (!_events.ContainsKey(eventType)) return;
            (_events[eventType] as Action)?.Invoke();
        }
    }
}