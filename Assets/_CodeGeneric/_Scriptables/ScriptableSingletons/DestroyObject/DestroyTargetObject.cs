using UnityEngine;

namespace _Code._Scriptables.ScriptableSingletons.DestroyObject {
	[CreateAssetMenu(menuName = "ScriptableSingletons/DestroyObject")]
	public class DestroyTargetObject : ScriptableObject 
	{
		public void DestroyTarget(GameObject gameObject)
		{
			Destroy(gameObject);
		}
	}
}
