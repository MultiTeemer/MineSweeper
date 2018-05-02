using UnityEngine;

namespace Assets.Scripts.Utils
{
	public static class GameObjectExtensions
	{
		public static GameObject Get(this GameObject obj, string name)
		{
			for (int i = 0; i < obj.transform.childCount; ++i) {
				var child = obj.transform.GetChild(i).gameObject;
				if (child.name == name) {
					return child;
				}

				var searchRes = child.Get(name);
				if (searchRes != null) return searchRes;
			}

			return null;
		}

		public static TComponent Get<TComponent>(this GameObject obj, string name)
			where TComponent : Component
		{
			return obj.Get(name).gameObject.GetComponent<TComponent>();
		}
	}
}
