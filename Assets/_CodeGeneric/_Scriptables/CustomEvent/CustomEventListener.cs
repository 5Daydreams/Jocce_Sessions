using UnityEngine;
using UnityEngine.Events;

namespace _Code._Scriptables.CustomEvent
{
    public class CustomEventListener<T,Event, UnityEventResponse> : MonoBehaviour, ICustomEventListener<T> where Event : CustomEventScriptable<T> where UnityEventResponse : UnityEvent<T>
    {
        [SerializeField] private Event _eventToListen;
        public Event EventToListen => _eventToListen;
        [SerializeField] private UnityEventResponse _response;

        private void Awake()
        {
            if(_eventToListen == null) 
                return;
            
            _eventToListen.Register(this);
        }

        private void OnDestroy()
        {
            if(_eventToListen == null)
                return;
            
            _eventToListen.DeRegister(this);
        }

        public void PlayResponse(T argument)
        {
            _response?.Invoke(argument);
        }
    }

    public interface ICustomEventListener<T>
    {
        void PlayResponse(T argument);
    }
}