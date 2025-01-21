using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Core.Events
{
    public class EventManager : MonoBehaviour
    {
        private static EventManager _instance;
        public static EventManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("GameEventManager");
                    _instance = go.AddComponent<EventManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        private Dictionary<Enum, Action> _events;
        private Dictionary<Enum, Action<object>> _eventsWithData;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            InitializeEventSystem();
            DontDestroyOnLoad(gameObject);
        }

        private void InitializeEventSystem()
        {
            _events = new Dictionary<Enum, Action>();
            _eventsWithData = new Dictionary<Enum, Action<object>>();
        }

        public void Subscribe(Enum eventType, Action listener)
        {
            if (!_events.ContainsKey(eventType))
                _events[eventType] = null;
            _events[eventType] += listener;
        }

        public void Subscribe<T>(Enum eventType, Action<T> listener)
        {
            if (!_eventsWithData.ContainsKey(eventType))
                _eventsWithData[eventType] = null;
            _eventsWithData[eventType] += (data) => listener((T)data);
        }

        public void Unsubscribe(Enum eventType, Action listener)
        {
            if (_events.ContainsKey(eventType))
                _events[eventType] -= listener;
        }

        public void Unsubscribe<T>(Enum eventType, Action<T> listener)
        {
            if (_eventsWithData.ContainsKey(eventType))
                _eventsWithData[eventType] -= (data) => listener((T)data);
        }

        public void TriggerEvent(Enum eventType)
        {
            if (_events.ContainsKey(eventType))
                _events[eventType]?.Invoke();
        }

        public void TriggerEvent<T>(Enum eventType, T data)
        {
            if (_eventsWithData.ContainsKey(eventType))
                _eventsWithData[eventType]?.Invoke(data);
        }
    }
}