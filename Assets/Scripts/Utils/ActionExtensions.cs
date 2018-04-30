using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
