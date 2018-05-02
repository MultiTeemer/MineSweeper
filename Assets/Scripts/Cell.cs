using System;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts
{
	enum CellContent
	{
		Bomb,
		FreeSpace,
	}

	enum VisualState
	{
		Closed,
		Opened,
	}

	class CellComponent
	{
		public CellContent Content;
		public VisualState VisualState;
		public bool Marked;

		public CellComponent(CellContent content, VisualState visualState, bool marked)
		{
			Content = content;
			VisualState = visualState;
			Marked = marked;
		}
	}

	class Cell : MonoBehaviour, IPointerClickHandler
	{
		public CellComponent Component;

		public event Action LeftClick;
		public event Action RightClick;

		public void Init(CellComponent component)
		{
			Component = component;

			UpdateAppearance();
		}

		public void SetNearbyBombsCounter(int count)
		{
			var text = gameObject.Get<Text>("Counter");
			text.color = GetColorForCounter(count);
			text.text = count.ToString();
		}

		public void MakeBgRed()
		{
			gameObject.Get("FailedBg").SetActive(true);
		}

		public void ToggleMark()
		{
			Component.Marked = !Component.Marked;

			gameObject.Get("Mark").SetActive(Component.Marked);
		}

		public void ShowMistakenMark()
		{
			gameObject.Get("Bomb").SetActive(true);
			gameObject.Get("Cross").SetActive(true);
		}

		public void Open()
		{
			Component.VisualState = VisualState.Opened;
		}

		public void UpdateAppearance()
		{
			if (Component.VisualState == VisualState.Opened) {
				CustomizeOpenedCell();
			} else {
				CustomizedClosedCell();
			}
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left) {
				LeftClick.SafeInvoke();
			} else if (eventData.button == PointerEventData.InputButton.Right) {
				RightClick.SafeInvoke();
			}
		}

		private void CustomizeOpenedCell()
		{
			if (Component.Content == CellContent.Bomb) {
				gameObject.Get("Bomb").SetActive(true);
			} else {
				gameObject.Get("Counter").SetActive(true);
			}

			gameObject.Get("Mark").SetActive(false);
		}

		private void CustomizedClosedCell()
		{
			gameObject.Get("Mark").SetActive(Component.Marked);
		}

		private static Color GetColorForCounter(int count)
		{
			switch (count) {
				case 1:
					return Color.blue;
				case 2:
					return Color.green;
				case 3:
					return Color.red;
				default:
					return Color.magenta;
			}
		}
	}
}
