﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
    public Text lapTimeText, countDownText;
    public GameObject raceFinishedScreen;
    public Text raceFinishedLapTimesText;

    int raceLapCount;
    string lapTimeString;
    bool updateLapTime = false;
    CarLapSystem playerLapSystem;
    List<CarLapSystem> finishedCars = new List<CarLapSystem>();

    public void Init(PlayerCar player, GameController gm) {
        playerLapSystem = player.lapSystem;
        raceLapCount = gm.raceLapCount;

        player.lapSystem.OnLapFinishedEvent.AddListener(StartLapTimer);
        gm.OnCarFinishedRaceEvent.AddListener(OnCarFinishedRace);
    }

    void Update() {
        UpdateLapText();
    }

    void StartLapTimer(CarLapSystem lapSystem) {
        updateLapTime = true;
    }

    void UpdateLapText() {
        if (!updateLapTime) return;

        float currentLapTime = Mathf.Round(playerLapSystem.currentLapTime * 100) / 100;
        float roundBestLapTime = Mathf.Round(playerLapSystem.bestLapTime * 100) / 100;
        string bestLapTimeText = "---";

        if (playerLapSystem.lap > 1)
            bestLapTimeText = "" + roundBestLapTime;

        lapTimeString = string.Format("Lap: {0} / {1}\n<color=white>{2}</color>\n<color=lime>Best: {3}</color>", playerLapSystem.lap, raceLapCount, currentLapTime, bestLapTimeText);

        lapTimeText.text = lapTimeString;
    }

    public void SetCountdownText(string text) {
        countDownText.text = text;
    }

    public void OnCarFinishedRace(CarLapSystem lapSystem) {
        //show race finished screen
        if (lapSystem.GetComponent<PlayerCar>()) {
            raceFinishedScreen.gameObject.SetActive(true);
            lapTimeText.gameObject.SetActive(false);
        }

        //update finished cars text
        finishedCars.Add(lapSystem);

        string text = "";
        for (int i = 0; i < finishedCars.Count; i++) {
            string text2 = string.Format("{0}.  -  {1}  -  Best time: {2:0.00}",
                i + 1,
                finishedCars[i].GetComponent<CarInfo>().CarDeveloperName,
                finishedCars[i].bestLapTime
                );

            if (finishedCars[i].GetComponent<PlayerCar>())
                text2 = "* " + text2 + " *";

            if (i < finishedCars.Count -1) text2 += "\n";
            text += text2;
        }

        raceFinishedLapTimesText.text = text;
    }
}
