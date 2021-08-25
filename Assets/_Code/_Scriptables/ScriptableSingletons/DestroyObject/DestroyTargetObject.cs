using UnityEngine;

namespace FG {
	[CreateAssetMenu(menuName = "ScriptableSingletons/DestroyObject")]
	public class DestroyTargetObject : ScriptableObject 
	{
		public void DestroyTarget(GameObject gameObject)
		{
			Destroy(gameObject);
		}
	}
}
