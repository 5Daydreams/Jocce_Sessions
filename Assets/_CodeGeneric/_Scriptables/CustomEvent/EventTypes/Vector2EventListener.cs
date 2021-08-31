using System;
using UnityEngine.Events;

namespace _Code._Scriptables.CustomEvent.EventTypes
{
    public class Vector2EventListener : CustomEventListener<Void, VoidEventScriptable, UnityVoidEvent> { }

    [Serializable] public class UnityVector2Event : UnityEvent<Void> { }
}