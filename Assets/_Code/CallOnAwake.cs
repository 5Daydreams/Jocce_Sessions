using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CallOnAwake : MonoBehaviour
{
    [SerializeField] private UnityEvent _event;
    void Awake()
    {
        _event.Invoke();
    }
}
