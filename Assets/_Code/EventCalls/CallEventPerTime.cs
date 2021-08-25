using System;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace _Code.EventCalls
{
    public class CallEventPerTime : MonoBehaviour
    {
        [SerializeField] private bool _startWithOffset = false;
        [SerializeField] private float _eventCooldown;
        [SerializeField] private UnityEvent _triggerWhenDone;
        
        private float _currentTime = 0;
        private bool _isActive = false;


        public void RestartRunningTimer()
        {
            _currentTime = 0;

            if (_startWithOffset)
                _currentTime += Random.Range(0.00f, _eventCooldown*0.9f);
                    
            ResumeTimer();
        }

        public void StopTimer()
        {
            _isActive = false;
        }

        public void ResumeTimer()
        {
            _isActive = true;
        }

        private void Update() => RunTimerLogic();

        private void RunTimerLogic()
        {
            if(!_isActive)
                return;

            if (_currentTime < _eventCooldown)
            {
                _currentTime += Time.deltaTime;
                return;
            }
            
            _triggerWhenDone.Invoke();
            _currentTime = 0;
        }
    }
}