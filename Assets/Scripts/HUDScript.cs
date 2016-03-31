using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDScript : MonoBehaviour {

	public Text hudText;

	string lapString;
	string positionString;

	public int lap;
	public int maxLaps;

	public int position = 1;
	int amountOfCars;

	float currentLapTime;
	float bestLapTime = Mathf.Infinity;
	string lapTimeString;

	GameObject[] allNPC;

	GameManager gm;

	void Start () {
		lapString = "Lap " + lap;
		gm = GameObject.Find("GameManager").GetComponent<GameManager>();
		allNPC = GameObject.FindGameObjectsWithTag("NPC");
		amountOfCars = allNPC.Length + 1;
	}

	void Update () {
		if (gm.gameStart) {
			//PlayerPositionUpdate();
			LapTime();
		}
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

	/*
	public void PlayerPositionUpdate () {

		position = 1;
		PlayerController pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
		for (int i = 0; i < allNPC.Length; i++) {
			NavMeshAgentController agentCont = allNPC[i].GetComponent<NavMeshAgentController>();
			if (lap < agentCont.GetCurrentLap()) {
				position++;
			} else if (pc.GetNodeIndex() < agentCont.GetNodeIndex()) {
				position++;
			} else if (pc.GetNodeIndex() == agentCont.GetNodeIndex() && pc.GetDistanceToNode() > agentCont.GetDistanceToNode()) {
				position++;
			} else {
				position--;
			}
		}

		position = Mathf.Clamp(position, 1, amountOfCars);

		if (position == 1) {
			positionString = "" + position + "st";
		} else if (position == 2) {
			positionString = "" + position + "nd";
		} else if (position == 3) {
			positionString = "" + position + "rd";
		} else {
			positionString = "" + position + "th";
		}
		TextUpdate();
	}
	*/

	void TextUpdate () {
		//hudText.text = lapString + "\n" + positionString;
		hudText.text = lapString + "\n" + lapTimeString;
	}
}
