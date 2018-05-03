using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameManagerStuff
{
	static class Cheats
	{
		public static bool Enabled { get; set; }

		public static void LastTurnToWin(Cell[,] cells)
		{
			var opts = GameManager.Instance.Options;
			var bombsPos = new List<Vector2Int>(opts.BombsCount);
			for (int i = 0; i < opts.FieldSize.x; ++i) {
				for (int j = 0; j < opts.FieldSize.y; ++j) {
					if (cells[i, j].Component.Content == CellContent.Bomb) {
						bombsPos.Add(new Vector2Int(i, j));
					}
				}
			}

			var lastBombToTakePos = bombsPos[Random.Range(0, opts.BombsCount)];
			for (int i = 0; i < opts.FieldSize.x; ++i) {
				for (int j = 0; j < opts.FieldSize.y; ++j) {
					cells[i, j].SetMarked(false);

					if (i == lastBombToTakePos.x && j == lastBombToTakePos.y) {
						cells[i, j].Component.VisualState = VisualState.Closed;
						cells[i, j].SetMarked(false);
					} else {
						if (cells[i, j].Component.Content == CellContent.Bomb) {
							cells[i, j].SetMarked(true);
						} else {
							cells[i, j].Open();
						}
					}

					cells[i, j].UpdateAppearance();

				}
			}

			GameManager.Instance.MarksSet = opts.BombsCount - 1;
		}
	}
}
