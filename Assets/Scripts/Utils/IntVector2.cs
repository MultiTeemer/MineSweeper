namespace Assets.Scripts.Utils
{
	struct IntVector2
	{
		public readonly int X;

		public readonly int Y;

		public IntVector2(int x, int y)
		{
			X = x;
			Y = y;
		}

		public static IntVector2 operator +(IntVector2 lhs, IntVector2 rhs)
		{
			return new IntVector2(lhs.X + rhs.X, lhs.Y + rhs.Y);
		}
	}
}
