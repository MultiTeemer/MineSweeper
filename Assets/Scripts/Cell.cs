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

		public void SetText(string text)
		{
			gameObject.Get<Text>("Counter").text = text;
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

		public void Open()
		{
			Component.VisualState = VisualState.Opened;

			if (Component.Content == CellContent.Bomb) {
				MakeBgRed();
			}
		}

		public void UpdateAppearance()
		{
			if (Component.VisualState == VisualState.Opened) {
				CustomizeOpenedCell();
			} else {
				CustomizedClosedCell();
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

		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left) {
				LeftClick.SafeInvoke();
			} else if (eventData.button == PointerEventData.InputButton.Right) {
				RightClick.SafeInvoke();
			}
		}
	}
}
