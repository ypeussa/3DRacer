using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using System.Collections;

public class GameManager : MonoBehaviour {
    public List<PlayerController> playerCarPrefabs;
    public List<NavMeshAgentController> AICarPrefabs;
    public GameObject carSelectionMenu;
    public Transform spawnPoint;
    public Transform cameraStart;
    public Transform carModelsParent;
    public HUDScript HUD;
    public float gameStartCamSize;
    public float avoidanceDistance;
    public float timeScale;
    public Text madeBy;

    int selectedCarIndex;
    CameraScript mainCam;
    Vector3 selectionCarPositionOffset = new Vector3(15, 0, 0);
    List<PlayerController> selectionMenuCars;
    List<NavMeshAgentController> AICars;
    NodeScript[] nodes;
    GameObject[] spawns;
    PlayerController player;

    public bool gameStart { get; private set; }

    void Start() {
        nodes = FindObjectsOfType<NodeScript>();
        spawns = GameObject.FindGameObjectsWithTag("NPCSpawn");
        mainCam = FindObjectOfType<CameraScript>();

        CreateSelectionCarModels();

        UpdateMadeByText();
    }

    void CreateSelectionCarModels() {
        selectionMenuCars = new List<PlayerController>();

        for (int i = 0; i < playerCarPrefabs.Count; i++) {
            selectionMenuCars.Add(Instantiate(playerCarPrefabs[i], carModelsParent));
            selectionMenuCars[i].SetAsSelectionMenuCar(selectionCarPositionOffset * i, Quaternion.LookRotation(new Vector3(-0.5f, -0.2f, -1)));
        }
    }

    void CreateAICars() {
        var npcParent = GameObject.Find("NPCs").transform;
        AICars = new List<NavMeshAgentController>();

        for (int i = 0; i < spawns.Length; i++) {
            if (i < AICarPrefabs.Count) {
                var npc = Instantiate(AICarPrefabs[i], spawns[i].transform.position, AICarPrefabs[i].transform.rotation);
                npc.name = "NPC" + i;
                npc.transform.SetParent(npcParent);
                AICars.Add(npc);
            }
        }
    }

    public void StartGame() {
        //disable selection menu
        carSelectionMenu.SetActive(false);
        carModelsParent.gameObject.SetActive(false);

        //spawn AI cars and the player car
        CreateAICars();
        player = Instantiate(playerCarPrefabs[selectedCarIndex], spawnPoint.position, Quaternion.identity);

        //camera setup
        mainCam.Init(cameraStart.position, cameraStart.rotation, gameStartCamSize, player);

        //node init
        for (int i = 0; i < nodes.Length; i++) {
            nodes[i].Init(player);
        }

        //AI car init
        for (int i = 0; i < AICars.Count; i++) {
            AICars[i].SetNextPath();
        }

        HUD.StartLapTime();

        StartCoroutine(CountDownCoroutine());
    }

    private IEnumerator CountDownCoroutine() {

        yield return new WaitForSeconds(1f);

        yield return new WaitForSeconds(1f);

        yield return new WaitForSeconds(1f);


    }

    //input 
    void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene("Main");
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    //selection menu events

    public void Next() {
        if (selectedCarIndex < selectionMenuCars.Count - 1) {
            carModelsParent.transform.position -= selectionCarPositionOffset;
            selectedCarIndex++;
            UpdateMadeByText();
        }
    }

    public void Previous() {
        if (selectedCarIndex > 0) {
            selectedCarIndex--;
            carModelsParent.transform.position += selectionCarPositionOffset;
            UpdateMadeByText();
        }
    }

    private void UpdateMadeByText() {
        madeBy.text = "Made by " + selectionMenuCars[selectedCarIndex].CarDeveloperName;
    }
}
