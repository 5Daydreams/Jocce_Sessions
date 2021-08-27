using UnityEngine;

namespace _Code._Scriptables.CustomEvent.EventTypes
{
    [CreateAssetMenu(menuName = "ScriptableEvents/VoidEvent",fileName = "Void Event")]
    public class VoidEventScriptable : CustomEventScriptable<Void>
    {
        public void Trigger() => Trigger(new Void());
    }
}