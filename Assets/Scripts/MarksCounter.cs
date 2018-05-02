using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class MarksCounter : MonoBehaviour
{
	private Text field;

	private void Start()
	{
		field = GetComponent<Text>();
	}
	
	private void Update()
	{
		var gm = GameManager.Instance;
		if (gm.IsGameRunning) {
			var count = gm.Options.BombsCount - gm.MarksSet;
			field.text = count.ToString();
		}
	}
}
