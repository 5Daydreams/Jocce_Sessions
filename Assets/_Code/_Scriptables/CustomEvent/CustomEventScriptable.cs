using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Code._Scriptables.CustomEvent
{
    public class CustomEventScriptable<T> : ScriptableObject
    {
        private readonly List<ICustomEventListener<T>> _listeners = new List<ICustomEventListener<T>>();

        public void Trigger(T argument)
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                _listeners[i].PlayResponse(argument);
            }
        }

        public void Register(ICustomEventListener<T> eventListener)
        {
            if(_listeners.Contains(eventListener))
                return;
            
            _listeners.Add(eventListener);
        }
        
        public void DeRegister(ICustomEventListener<T> eventListener)
        {
            if(!_listeners.Contains(eventListener))
                return;
            
            _listeners.Remove(eventListener);        }
    }
    
    [Serializable] public struct Void { }
}