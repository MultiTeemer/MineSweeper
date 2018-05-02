using System;

namespace Assets.Scripts.Utils
{
	public static class ActionExtensions
	{
		public static void SafeInvoke(this Action action)
		{
			if (action != null) {
				action();
			}
		}
	}
}
