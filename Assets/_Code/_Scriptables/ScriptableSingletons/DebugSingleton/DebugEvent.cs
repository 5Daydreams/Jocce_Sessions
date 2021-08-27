using UnityEngine;

namespace _Code._Scriptables.ScriptableSingletons.DebugSingleton
{
    [CreateAssetMenu(menuName = "ScriptableSingletons/Debug")]
    public class DebugEvent : ScriptableObject
    {
        public void DefaultDebugMessage()
        {
            Debug.Log("All your BASE are Belong TO US");
        }
        
        public void DebugStringMessage(string message)
        {
            Debug.Log(message);
        }
    }
}