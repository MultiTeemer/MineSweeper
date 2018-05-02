using UnityEngine;

namespace Assets.Scripts.Utils
{
	public static class Vector2Extensions
	{
		public static Vector2 Multiply(Vector2 lhs, Vector2 rhs)
		{
			return new Vector2(lhs.x * rhs.x, lhs.y * rhs.y);
		}
	}
}
