using _Code._Scriptables.CustomEvent.EventTypes;
using UnityEngine;

namespace _Code.Grid.ElementTypes
{
    [RequireComponent(typeof(Collider2D))]
    public class GridExit : MonoBehaviour
    {
        [SerializeField] private VoidEventScriptable _callOnExitReached;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player")) 
                return;
            
            _callOnExitReached.Trigger();
            Destroy(this.gameObject);
        }
    }
}