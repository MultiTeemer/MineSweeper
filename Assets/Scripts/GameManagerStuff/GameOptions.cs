using System;
using UnityEngine;

namespace Assets.Scripts.GameManagerStuff
{
	[Serializable]
	public struct GameOptions
	{
		public Vector2Int FieldSize;
		public int BombsCount;

		public static GameOptions EasyMode = new GameOptions(new Vector2Int(10, 10), 10);
		public static GameOptions MediumMode = new GameOptions(new Vector2Int(20, 20), 40);
		public static GameOptions HardMode = new GameOptions(new Vector2Int(30, 30), 90);

		public GameOptions(Vector2Int fieldSize, int bombsCount)
		{
			FieldSize = fieldSize;
			BombsCount = bombsCount;
		}
	}
}
