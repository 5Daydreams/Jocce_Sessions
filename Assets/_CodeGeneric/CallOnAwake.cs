using UnityEngine;
using UnityEngine.Events;

namespace _Code
{
    public class CallOnAwake : MonoBehaviour
    {
        [SerializeField] private UnityEvent _event;
        void Awake()
        {
            _event.Invoke();
        }
    }
}
