using System;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts
{
	[Serializable]
	public class CellData
	{
		public bool Bomb;
		public bool Opened;
		public bool Marked;

		public CellData(bool bomb, bool opened, bool marked)
		{
			Bomb = bomb;
			Opened = opened;
			Marked = marked;
		}
	}

	class Cell : MonoBehaviour, IPointerClickHandler
	{
		public CellData Data;

		public event Action LeftClick;
		public event Action RightClick;
		public event Action Opened;

		public void Init(CellData data)
		{
			Data = data;

			UpdateAppearance();
		}

		public void SetNearbyBombsCounter(int count)
		{
			var text = gameObject.Get<Text>("Counter");
			text.color = GetColorForCounter(count);
			text.text = count == 0 ? "*" : count.ToString();
		}

		public void MakeBgRed()
		{
			gameObject.Get("FailedBg").SetActive(true);
		}

		public void SetMarked(bool value)
		{
			Data.Marked = value;

			gameObject.Get("Mark").SetActive(value);
		}

		public void ShowMistakenMark()
		{
			gameObject.Get("Bomb").SetActive(true);
			gameObject.Get("Cross").SetActive(true);
		}

		public void Open()
		{
			Data.Opened = true;;

			Opened.SafeInvoke();
		}

		public void UpdateAppearance()
		{
			if (Data.Opened) {
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
			if (Data.Bomb) {
				gameObject.Get("Bomb").SetActive(true);
			} else {
				gameObject.Get("Counter").SetActive(true);
			}

			gameObject.Get("Mark").SetActive(false);
		}

		private void CustomizedClosedCell()
		{
			gameObject.Get("Mark").SetActive(Data.Marked);
			gameObject.Get("Counter").SetActive(false);
			gameObject.Get("Bomb").SetActive(false);
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
