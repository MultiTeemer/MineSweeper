using UnityEngine;

namespace Assets.Scripts.GameManagerStuff
{
	public class InterfaceController : MonoBehaviour
	{
		public Canvas MainMenuCanvas;
		public Canvas GameCanvas;
		public Canvas ChooseGameModeCanvas;

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

		private void StartGame(GameOptions options)
		{
			SwitchTo(GameCanvas);

			GameManager.Instance.StartGame(options);
		}

		private void SwitchTo(Canvas canvas)
		{
			if (currentActiveCanvas != null) {
				HideCanvas(currentActiveCanvas);
			}

			ShowCanvas(canvas);
		}

		private void Start()
		{
			HideCanvas(GameCanvas);
			HideCanvas(ChooseGameModeCanvas);

			ShowCanvas(MainMenuCanvas);
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
