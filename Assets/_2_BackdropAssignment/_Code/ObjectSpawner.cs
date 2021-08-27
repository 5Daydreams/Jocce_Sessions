using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FG {
	public class ObjectSpawner : MonoBehaviour
	{
		[SerializeField] private List<SpawnInfo> _spawnList;

		public void SpawnByKey(string IdToSpawn)
		{
			foreach (var spawnInfo in _spawnList)
			{
				if (spawnInfo.ObjectId == IdToSpawn)
					spawnInfo.SpawnThis();
			}
		}

		private GameObject SpawnObject(SpawnInfo info)
		{
			return Instantiate(info.ObjectToSpawn, info.Position,info.Rotation);
		}

	}
	
	[Serializable] public struct SpawnInfo
	{
		public string ObjectId;
		public GameObject ObjectToSpawn;
		public Vector3 Position;
		public Quaternion Rotation;

		public SpawnInfo(string name, GameObject gameObject)
		{
			ObjectId = name;
			ObjectToSpawn = gameObject;
			Position = gameObject.transform.position;
			Rotation = gameObject.transform.rotation;
		}

		public GameObject SpawnThis()
		{
			return Object.Instantiate(ObjectToSpawn,Position,Rotation);
		}
	}
}
