using Assets.Scripts.Utils;

namespace Assets.Scripts.GameManagerStuff
{
	public struct GameOptions
	{
		public IntVector2 FieldSize;
		public int BombsCount;

		public static GameOptions EasyMode = new GameOptions(new IntVector2(10), 10);
		public static GameOptions MediumMode = new GameOptions(new IntVector2(20), 40);
		public static GameOptions HardMode = new GameOptions(new IntVector2(30), 90);

		public GameOptions(IntVector2 fieldSize, int bombsCount)
		{
			FieldSize = fieldSize;
			BombsCount = bombsCount;
		}
	}
}
