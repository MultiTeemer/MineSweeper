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
		timePassed = Time.deltaTime;
	}
	
	private void Update()
	{
		timePassed += Time.deltaTime;
		text.text = TimeSpan.FromSeconds(timePassed).ToString();
	}
}
