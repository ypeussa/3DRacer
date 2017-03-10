using UnityEngine;
using UnityEngine.UI;

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

    void Start() {
        lapString = "Lap " + lap;
        allNPC = GameObject.FindGameObjectsWithTag("NPC");
        amountOfCars = allNPC.Length + 1;
    }

    void Update() {
        if (updateLapTime)
            UpdateLapText();
    }

    public void RecordLap() {
        lap++;
        lapString = "Lap " + lap;
        if (currentLapTime < bestLapTime && lap > 1) {
            bestLapTime = currentLapTime;
        }
        currentLapTime = 0;
    }

    void UpdateLapText() {
        currentLapTime += Time.deltaTime;
        float roundCurrentLapTime = Mathf.Round(currentLapTime * 100) / 100;
        float roundBestLapTime = Mathf.Round(bestLapTime * 100) / 100;
        string roundBestLapTimeText = "---";

        if (lap > 1) {
            roundBestLapTimeText = "" + roundBestLapTime;

        }
        lapTimeString =
            "<color=white>" + roundCurrentLapTime + "</color>\n" +
        "<color=lime>Best: " + roundBestLapTimeText + "</color>";

        lapTimeText.text = lapString + "\n" + lapTimeString;
    }

    public void StartLap() {
        updateLapTime = true;
    }

    public void SetCountdownText(string text) {
        countDownText.text = text;
    }
}
