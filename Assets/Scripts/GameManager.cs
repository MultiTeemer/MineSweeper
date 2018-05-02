using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Assets.Scripts;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
	public Canvas MainCanvas;
	public Canvas GameCanvas;
	public RectTransform FieldOrigin;
	public Text Timer;
	public Text Bombs;

	public GameObject CellPrefab;

	private Cell[,] cells;
	private GameObject field;

	public void StartGame()
	{
		SetMainCanvasActive(false);
		SetGameCanvasActive(true);
		
		InitField(new IntVector2(10, 10), 10);
	}

	private void Start()
	{
		field = GameCanvas.gameObject.Get("Field");

		SetMainCanvasActive(true);
		SetGameCanvasActive(false);
	}

	private void Update()
	{
		
	}

	private void SetMainCanvasActive(bool value)
	{
		MainCanvas.gameObject.SetActive(value);
	}

	private void SetGameCanvasActive(bool value)
	{
		GameCanvas.gameObject.SetActive(value);
	}

	private void InitField(IntVector2 size, int bombsCount)
	{
		cells = new Cell[size.X, size.Y];

		var bombPositions = new List<IntVector2>(bombsCount);
		for (int i = 0; i < bombsCount; ++i) {
			Func<IntVector2> getRandomPos = () => new IntVector2(Random.Range(0, size.X), Random.Range(0, size.Y));
			var c = getRandomPos();
			while (bombPositions.Contains(c)) {
				c = getRandomPos();
			}
			bombPositions.Add(c);
		}

		var cellSize = CellPrefab.GetComponent<RectTransform>().sizeDelta;
		for (int i = 0; i < size.X; ++i) {
			for (int j = 0; j < size.Y; ++j) {
				var c = new IntVector2(i, j);
				var content = bombPositions.Contains(c) ? CellContent.Bomb : CellContent.FreeSpace;

				var cellObj = Object.Instantiate(CellPrefab, field.transform);
				cellObj.transform.localPosition = new Vector3(i * cellSize.x, j * cellSize.y);

				var cell = cellObj.GetComponent<Cell>();
				cells[i, j] = cell;

				cell.Init(new CellComponent(content, VisualState.Closed, false));
				cell.LeftClick += () => CellLeftClick(c);
				cell.RightClick += () => CellRightClick(c);
			}
		}

		for (int i = 0; i < size.X; ++i) {
			for (int j = 0; j < size.Y; j++) {
				var bombsCountNearby = CalcBombsNearbyCount(new IntVector2(i, j));
				cells[i, j].SetText(bombsCountNearby == 0 ? "0" : bombsCountNearby.ToString());
			}
		}
	}

	private void CellLeftClick(IntVector2 c)
	{
		var cell = cells[c.X, c.Y];
		if (cell.Component.VisualState == VisualState.Closed) {
			if (cell.Component.Content == CellContent.Bomb) {
				cell.MakeBgRed();
				cell.Component.VisualState = VisualState.Opened;
			} else if (CalcBombsNearbyCount(c) > 0) {
				cell.Component.VisualState = VisualState.Opened;
			} else {
				OpenSafeArea(c);
			}

			cell.UpdateAppearance();
		}
	}

	private int CalcBombsNearbyCount(IntVector2 c)
	{
		Func<int, int, int> bombValue = (x, y) => {
			var cell = GetCell(new IntVector2(x, y));

			return cell != null && cell.Component.Content == CellContent.Bomb
				? 1
				: 0;
		};

		int i = c.X;
		int j = c.Y;
		var bombsCountNearby = bombValue(i - 1, j - 1)
			+ bombValue(i, j - 1)
			+ bombValue(i + 1, j - 1)
			+ bombValue(i - 1, j)
			+ bombValue(i + 1, j)
			+ bombValue(i - 1, j + 1)
			+ bombValue(i, j + 1)
			+ bombValue(i + 1, j + 1);

		return bombsCountNearby;
	}

	private void CellRightClick(IntVector2 c)
	{
		cells[c.X, c.Y].ToggleMark();
	}

	private void OpenSafeArea(IntVector2 c)
	{
		var cellsQueue = new Queue<IntVector2>();
		cellsQueue.Enqueue(c);

		Action<IntVector2> pushIfSafe = (v) => {
			var cell = GetCell(v);
			if (cell != null && cell.Component.Content == CellContent.FreeSpace && cell.Component.VisualState == VisualState.Closed) {
				cellsQueue.Enqueue(v);
			}
		};
		var shifts = new List<IntVector2> {
			new IntVector2(-1, 0),
			new IntVector2(1, 0),
			new IntVector2(0, -1),
			new IntVector2(0, 1),
		};
		while (cellsQueue.Count > 0) {
			var currCoord = cellsQueue.Dequeue();
			var cell = GetCell(currCoord);
			cell.Component.VisualState = VisualState.Opened;
			cell.UpdateAppearance();

			if (CalcBombsNearbyCount(currCoord) == 0) {
				foreach (var shift in shifts) {
					pushIfSafe(currCoord + shift);
				}
			}
		}
	}

	private Cell GetCell(IntVector2 c)
	{
		return c.X >= 0
				&& c.X < cells.GetLength(0)
				&& c.Y >= 0
				&& c.Y < cells.GetLength(1)
			? cells[c.X, c.Y]
			: null;
	}
}
