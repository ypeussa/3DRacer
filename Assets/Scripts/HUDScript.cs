using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDScript : MonoBehaviour {

	public Text lapTimeText, countDownText;

	string lapString;
	string positionString;

	public int lap;
	public int maxLaps;

	public int position = 1;
	int amountOfCars;

	float currentLapTime;
	float bestLapTime = Mathf.Infinity;
	string lapTimeString;
    bool updateLapTime = false;

	GameObject[] allNPC;

	void Start () {
		lapString = "Lap " + lap;
		allNPC = GameObject.FindGameObjectsWithTag("NPC");
		amountOfCars = allNPC.Length + 1;
	}

	void Update () {
        if (updateLapTime)
            LapTime();
	}

	public void LapUpdate () {
		lap++;
		lapString = "Lap " + lap;
		if (currentLapTime < bestLapTime && lap > 1) {
			bestLapTime = currentLapTime;
		}
		currentLapTime = 0;
		TextUpdate();
	}

	void LapTime () {
		currentLapTime += Time.deltaTime;
		float roundCurrentLapTime = Mathf.Round(currentLapTime * 100) / 100;
		float roundBestLapTime = Mathf.Round(bestLapTime * 100) / 100;
		if (lap < 2) {
			lapTimeString = "<color=lime>Best: ---</color>" + "\n<color=white>Current: " + roundCurrentLapTime + "</color>";
		} else {
			lapTimeString = "<color=lime>Best: " + roundBestLapTime + "</color>\n<color=white>Current: " + roundCurrentLapTime + "</color>";
		}
		TextUpdate();
	}

    public void StartLapTime() {
        updateLapTime = true;
    }

	void TextUpdate () {
		lapTimeText.text = lapString + "\n" + lapTimeString;
	}
}
