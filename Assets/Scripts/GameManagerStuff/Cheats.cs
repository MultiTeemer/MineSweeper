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
			var safeCells = new List<Vector2Int>(opts.BombsCount);
			for (int i = 0; i < opts.FieldSize.x; ++i) {
				for (int j = 0; j < opts.FieldSize.y; ++j) {
					if (cells[i, j].Component.Content == CellContent.FreeSpace) {
						safeCells.Add(new Vector2Int(i, j));
					}
				}
			}

			var lastSafeCell = safeCells[Random.Range(0, safeCells.Count)];
			for (int i = 0; i < opts.FieldSize.x; ++i) {
				for (int j = 0; j < opts.FieldSize.y; ++j) {
					var cell = cells[i, j];
					cell.SetMarked(false);

					if (i == lastSafeCell.x && j == lastSafeCell.y) {
						cell.Component.VisualState = VisualState.Closed;
						cell.SetMarked(false);
					} else {
						if (cell.Component.Content == CellContent.Bomb) {
							cell.SetMarked(true);
						} else {
							cell.Open();
						}
					}

					cell.UpdateAppearance();

				}
			}

			GameManager.Instance.MarksSet = opts.BombsCount;
			GameManager.Instance.CellsOpened = opts.FieldSize.x * opts.FieldSize.y - opts.BombsCount - 1;
		}
	}
}
