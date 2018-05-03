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

	[HideInInspector]
	public int CellsOpened;

	private Cell[,] cells;

	public static GameManager Instance { get; private set; }

	public event Action GameWon;
	public event Action GameLost;

	public void StartGame(GameOptions options)
	{
		Options = options;
		IsGameRunning = true;
		MarksSet = 0;
		CellsOpened = 0;

		InitField(options.FieldSize, options.BombsCount);
	}

	public void StopGame()
	{
		for (int i = 0; i < Options.FieldSize.x; ++i) {
			for (int j = 0; j < Options.FieldSize.y; ++j) {
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

	private void InitField(Vector2Int size, int bombsCount)
	{
		cells = new Cell[size.x, size.y];

		var bombPositions = new List<Vector2Int>(bombsCount);
		for (int i = 0; i < bombsCount; ++i) {
			Func<Vector2Int> getRandomPos = () => new Vector2Int(Random.Range(0, size.x), Random.Range(0, size.y));
			var c = getRandomPos();
			while (bombPositions.Contains(c)) {
				c = getRandomPos();
			}
			bombPositions.Add(c);
		}

		var cellSize = Vector2Extensions.Multiply(CellPrefab.GetComponent<RectTransform>().sizeDelta, CellPrefab.GetComponent<RectTransform>().localScale);
		var wholeFieldSize = new Vector2(cellSize.x * Options.FieldSize.x, cellSize.y * Options.FieldSize.y);
		var shift = new Vector3(-wholeFieldSize.x, -wholeFieldSize.y) / 2;
		for (int i = 0; i < size.x; ++i) {
			for (int j = 0; j < size.y; ++j) {
				var c = new Vector2Int(i, j);
				var bomb = bombPositions.Contains(c);

				var cellObj = Object.Instantiate(CellPrefab, Field.transform);
				cellObj.transform.localPosition = shift + new Vector3(i * cellSize.x, j * cellSize.y);

				var cell = cellObj.GetComponent<Cell>();
				cells[i, j] = cell;

				cell.Init(new CellComponent(bomb, false, false));
				cell.LeftClick += () => CellLeftClick(c);
				cell.RightClick += () => CellRightClick(c);
				cell.Opened += OnCellOpened;
			}
		}

		for (int i = 0; i < size.x; ++i) {
			for (int j = 0; j < size.y; j++) {
				var bombsCountNearby = CalcBombsNearbyCount(new Vector2Int(i, j));
				cells[i, j].SetNearbyBombsCounter(bombsCountNearby);
			}
		}
	}

	private void CellLeftClick(Vector2Int c)
	{
		if (!IsGameRunning) return;

		var cell = cells[c.x, c.y];
		if (!cell.Component.Opened) {
			if (cell.Component.Bomb) {
				LoseGame(cell);
			} else if (CalcBombsNearbyCount(c) > 0) {
				cell.Open();
			} else {
				OpenSafeArea(c);
			}

			cell.UpdateAppearance();
		}
	}

	private void CellRightClick(Vector2Int c)
	{
		if (!IsGameRunning) return;

		var cell = cells[c.x, c.y];
		if (!cell.Component.Opened) {
			if (!cell.Component.Marked && MarksSet < Options.BombsCount) {
				++MarksSet;
				cell.SetMarked(true);
			} else if (cell.Component.Marked) {
				cell.SetMarked(false);
				--MarksSet;
			}
		}
	}

	private void WinGame()
	{
		IsGameRunning = false;

		GameWon.SafeInvoke();
	}

	private void LoseGame(Cell failReason)
	{
		IsGameRunning = false;

		foreach (var cell in cells) {
			if (cell.Component.Bomb && !cell.Component.Opened) {
				cell.Open();
			}

			cell.UpdateAppearance();

			if (cell == failReason) {
				cell.MakeBgRed();
			}

			if (!cell.Component.Bomb && cell.Component.Marked) {
				cell.ShowMistakenMark();
			}
		}

		GameLost.SafeInvoke();
	}

	private void OnCellOpened()
	{
		if (IsGameRunning) {
			++CellsOpened;
		}
	}

	private int CalcBombsNearbyCount(Vector2Int c)
	{
		Func<int, int, int> bombValue = (x, y) => {
			var cell = GetCell(new Vector2Int(x, y));
			return Convert.ToInt32(cell != null && cell.Component.Bomb);
		};

		int i = c.x;
		int j = c.y;
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

	private void OpenSafeArea(Vector2Int c)
	{
		var cellsQueue = new Queue<Vector2Int>();
		cellsQueue.Enqueue(c);

		Action<Vector2Int> pushIfSafe = (v) => {
			var cell = GetCell(v);
			if (
				cell != null
				&& !cell.Component.Bomb
				&& !cell.Component.Opened
			) {
				cellsQueue.Enqueue(v);
			}
		};
		var shifts = new List<Vector2Int> {
			new Vector2Int(-1, 0),
			new Vector2Int(1, 0),
			new Vector2Int(0, -1),
			new Vector2Int(0, 1),
		};
		while (cellsQueue.Count > 0) {
			var currCoord = cellsQueue.Dequeue();
			var cell = GetCell(currCoord);
			cell.Open();
			cell.UpdateAppearance();

			if (CalcBombsNearbyCount(currCoord) == 0) {
				foreach (var shift in shifts) {
					pushIfSafe(currCoord + shift);
				}
			}
		}
	}

	private Cell GetCell(Vector2Int c)
	{
		return c.x >= 0
				&& c.x < cells.GetLength(0)
				&& c.y >= 0
				&& c.y < cells.GetLength(1)
			? cells[c.x, c.y]
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

		if (IsGameRunning) {
			if (CellsOpened == Options.FieldSize.x * Options.FieldSize.y - Options.BombsCount) {
				WinGame();
			}

#if UNITY_EDITOR
			if (Input.GetKey(KeyCode.W)) {
				Cheats.LastTurnToWin(cells);
			}
#endif
		}
	}
}
