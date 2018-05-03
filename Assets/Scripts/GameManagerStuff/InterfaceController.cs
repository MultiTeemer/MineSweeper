using System.Net.NetworkInformation;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameManagerStuff
{
	public class InterfaceController : MonoBehaviour
	{
		public GameObject SavedGamePrefab;

		public Canvas MainMenuCanvas;
		public Canvas GameCanvas;
		public Canvas ChooseGameModeCanvas;
		public Canvas LoadGameCanvas;

		private Canvas currentActiveCanvas;

		public static InterfaceController Instance { get; private set; }

		public void ShowCanvas(Canvas canvas)
		{
			canvas.gameObject.SetActive(true);
			currentActiveCanvas = canvas;
		}

		public void HideCanvas(Canvas canvas)
		{
			canvas.gameObject.SetActive(false);

			if (canvas == currentActiveCanvas) {
				currentActiveCanvas = null;
			}
		}

		public void NewGameBtnClicked()
		{
			SwitchTo(ChooseGameModeCanvas);
		}

		public void EaseModeBtnClicked()
		{
			StartGame(GameOptions.EasyMode);
		}

		public void MediumModeBtnClicked()
		{
			StartGame(GameOptions.MediumMode);
		}

		public void HardModeBtnClicked()
		{
			StartGame(GameOptions.HardMode);
		}

		public void ExitBtnClicked()
		{
			GameManager.Instance.StopGame();
			ReturnToMainMenu();
		}

		public void ReturnToMainMenu()
		{
			SwitchTo(MainMenuCanvas);
		}

		public void SaveBtnClicked()
		{
			GameManager.Instance.SaveGame();
		}

		public void LoadGameBtnClicked()
		{
			SwitchTo(LoadGameCanvas);

			InitSavedGamesList();
		}

		private void InitSavedGamesList()
		{
			var scrollView = LoadGameCanvas.gameObject.Get<RectTransform>("SavedGames");
			foreach (Transform c in scrollView.transform) {
				Destroy(c.gameObject);
			}

			var saves = LoadSaveSystem.GetSavedGames();
			var cnt = 0;
			var size = SavedGamePrefab.GetComponent<RectTransform>().sizeDelta;
			foreach (var path in saves) {
				var btn = Object.Instantiate(SavedGamePrefab, scrollView.transform);
				btn.Get<Text>("Text").text = path;
				btn.transform.localPosition = new Vector3(0, -cnt++ * size.y);
				var locPath = path;
				btn.GetComponent<Button>().onClick.AddListener(() => {
					GameManager.Instance.LoadGame(locPath);
					ShowGameCanvas();
				});
			}
		}

		private void StartGame(GameOptions options)
		{
			ShowGameCanvas();

			GameManager.Instance.StartGame(options);
		}

		private void ShowGameCanvas()
		{
			SwitchTo(GameCanvas);

			GameCanvas.gameObject.Get<Button>("SaveGameBtn").interactable = true;
			GameCanvas.gameObject.Get("WinMessage").SetActive(false);
			GameCanvas.gameObject.Get("LoseMessage").SetActive(false);
		}

		private void SwitchTo(Canvas canvas)
		{
			if (currentActiveCanvas != null) {
				HideCanvas(currentActiveCanvas);
			}

			ShowCanvas(canvas);
		}

		private void OnGameWon()
		{
			GameCanvas.gameObject.Get<Button>("SaveGameBtn").interactable = false;
			GameCanvas.gameObject.Get("WinMessage").SetActive(true);
		}

		private void OnGameLost()
		{
			GameCanvas.gameObject.Get<Button>("SaveGameBtn").interactable = false;
			GameCanvas.gameObject.Get("LoseMessage").SetActive(true);
		}

		private void Start()
		{
			HideCanvas(GameCanvas);
			HideCanvas(ChooseGameModeCanvas);
			HideCanvas(LoadGameCanvas);

			ShowCanvas(MainMenuCanvas);

			GameManager.Instance.GameWon += OnGameWon;
			GameManager.Instance.GameLost += OnGameLost;
		}

		private void Awake()
		{
			if (Instance != null && Instance != this) {
				Destroy(Instance);
				return;
			}

			Instance = this;
		}
	}
}
