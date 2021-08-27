using System;
using UnityEngine.Events;

namespace _Code._Scriptables.CustomEvent.EventTypes
{
    public class VoidEventListener : CustomEventListener<Void, VoidEventScriptable, UnityVoidEvent> { }

    [Serializable] public class UnityVoidEvent : UnityEvent<Void> { }
}