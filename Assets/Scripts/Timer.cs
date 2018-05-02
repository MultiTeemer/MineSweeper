using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class Timer : MonoBehaviour
{
	private Text text;
	private float timePassed;

	private void Start()
	{
		text = GetComponent<Text>();
	}

	private void OnEnable()
	{
		timePassed = 0;
	}
	
	private void Update()
	{
		if (GameManager.Instance.IsGameRunning) {
			timePassed += Time.deltaTime;
			text.text = TimeSpan.FromSeconds(timePassed).ToString();
		}
	}
}
