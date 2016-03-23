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

	GameObject[] allNPC;

	GameManager gm;

	void Start () {
		
		lapString = "Lap " + lap + " / " + maxLaps;
		gm = GameObject.Find("GameManager").GetComponent<GameManager>();
		allNPC = GameObject.FindGameObjectsWithTag("NPC");
		amountOfCars = allNPC.Length + 1;
	}

	void Update () {
		if (gm.gameStart) {
			//PlayerPositionUpdate();
		}
	}

	public void LapUpdate () {
		lap++;
		lapString = "Lap " + lap + " / " + maxLaps;
		TextUpdate();
	}

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

	void TextUpdate () {
		hudText.text = lapString + "\n" + positionString;
	}
}
