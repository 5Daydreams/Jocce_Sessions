using UnityEngine;

namespace _Code._Scriptables.ValueTypes
{
    public class BaseValueType<T> : ScriptableObject
    {
        public T Value;
        [TextArea] public string Description;
    }
}