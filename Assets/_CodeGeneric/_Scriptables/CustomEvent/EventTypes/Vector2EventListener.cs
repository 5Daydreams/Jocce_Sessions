using System;
using UnityEngine;
using UnityEngine.Events;

namespace _Code._Scriptables.CustomEvent.EventTypes
{
    public class Vector2EventListener : CustomEventListener<Vector2, Vector2EventScriptable, UnityVector2Event> { }

    [Serializable] public class UnityVector2Event : UnityEvent<Vector2> { }
}