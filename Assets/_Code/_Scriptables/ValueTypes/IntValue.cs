using UnityEngine;

namespace _Code.Scriptables
{
    [CreateAssetMenu(menuName = "ScriptableValues/Int", fileName = "IntValue")]
    public class IntValue : BaseValueType<int>
    {
        public void DecrementInt(int decrement)
        {
            Value -= decrement;
        }

        public void IncrementInt(int increment)
        {
            Value += increment;
        }

        public void SetIntValue( int overwriteValue)
        {
            Value = overwriteValue;
        }
        
    }

}