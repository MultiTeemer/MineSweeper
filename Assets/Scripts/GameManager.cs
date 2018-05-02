using System;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.GameManagerStuff;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
	public RectTransform FieldOrigin;
	public Text Timer;
	public Text Bombs;
	public GameObject CellPrefab;
	public GameObject Field;

	[HideInInspector]
	public GameOptions Options;

	[HideInInspector]
	public bool IsGameRunning;

	[HideInInspector]
	public int MarksSet;

	private Cell[,] cells;

	public static GameManager Instance { get; private set; }

	public void StartGame(GameOptions options)
	{
		Options = options;
		IsGameRunning = true;

		InitField(options.FieldSize, options.BombsCount);
	}

	public void StopGame()
	{
		for (int i = 0; i < Options.FieldSize.X; ++i) {
			for (int j = 0; j < Options.FieldSize.Y; ++j) {
				cells[i, j] = null;
			}
		}

		foreach (Transform c in Field.transform) {
			Destroy(c.gameObject);
		}

		IsGameRunning = false;
	}

	public void QuitGame()
	{
		Application.Quit();
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

				var cellObj = Object.Instantiate(CellPrefab, Field.transform);
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
				cells[i, j].SetNearbyBombsCounter(bombsCountNearby);
			}
		}
	}

	private void CellLeftClick(IntVector2 c)
	{
		if (!IsGameRunning) return;

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

	private void CellRightClick(IntVector2 c)
	{
		if (!IsGameRunning) return;

		var cell = cells[c.X, c.Y];

		if (!cell.Component.Marked && MarksSet < Options.BombsCount) {
			++MarksSet;
			cell.ToggleMark();
		} else if (cell.Component.Marked) {
			cell.ToggleMark();
			--MarksSet;
		}
	}

	private int CalcBombsNearbyCount(IntVector2 c)
	{
		Func<int, int, int> bombValue = (x, y) => {
			var cell = GetCell(new IntVector2(x, y));
			return Convert.ToInt32(cell != null && cell.Component.Content == CellContent.Bomb);
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

	private void Awake()
	{
		if (Instance != null && Instance != this) {
			Destroy(Instance);
			return;
		}

		Instance = this;
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.Escape)) {
			QuitGame();
		}
	}
}
