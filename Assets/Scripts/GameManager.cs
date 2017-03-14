using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
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

    Vector3 carModelsParentStartPosition;
    int selectedCarIndex;
    CameraScript mainCam;
    Vector3 selectionCarPositionOffset = new Vector3(15, 0, 0);
    List<PlayerController> selectionMenuCars;
    List<NavMeshAgentController> AICars;
    NodeScript[] nodes;
    GameObject[] spawns;
    PlayerController player;

    public int DEBUG_AmountOfAICars = 1000;

    public bool gameStart { get; private set; }

    void Start() {
        nodes = FindObjectsOfType<NodeScript>();
        spawns = GameObject.FindGameObjectsWithTag("NPCSpawn");
        mainCam = FindObjectOfType<CameraScript>();

        carModelsParentStartPosition = carModelsParent.transform.position;
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

    void CreateAICars(int skipCarIndex) {
        var npcParent = GameObject.Find("NPCs").transform;
        AICars = new List<NavMeshAgentController>();

        for (int i = 0; i < spawns.Length; i++) {
#if UNITY_EDITOR
            if (i == DEBUG_AmountOfAICars) break;
#endif


            if (i < AICarPrefabs.Count && i != skipCarIndex) {
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
        CreateAICars(selectedCarIndex);
        player = Instantiate(playerCarPrefabs[selectedCarIndex], spawnPoint.position, Quaternion.identity);

        //camera setup
        mainCam.Init(cameraStart.position, cameraStart.rotation, gameStartCamSize, player);

        //node init
        for (int i = 0; i < nodes.Length; i++) {
            nodes[i].Init(player);
        }

        player.enabled = false;
        StartCoroutine(CountDownCoroutine());
    }

    private IEnumerator CountDownCoroutine() {
        var countDownWait = new WaitForSeconds(0.8f);

        HUD.SetCountdownText("3");
        yield return countDownWait;
        HUD.SetCountdownText("2");
        yield return countDownWait;
        HUD.SetCountdownText("1");
        yield return countDownWait;
        HUD.SetCountdownText("GO!");
        HUD.StartLap();
        player.enabled = true;
        yield return countDownWait;
        HUD.SetCountdownText("");

        //start AI cars
        for (int i = 0; i < AICars.Count; i++) {
            AICars[i].StartMoving();
        }
    }

    //input 
    void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene("Main");
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
        //selection menu input
        if (carSelectionMenu.activeSelf) {
            if (Input.GetKeyDown(KeyCode.Return)) {
                StartGame();
            }

            //using axis for controller support

            float horizontalAxis = Input.GetAxisRaw("Horizontal");

            if (horizontalAxis != 0 && selectionInputTimer < Time.time) {
                if (horizontalAxis > 0) SelectNextCar();
                else SelectPreviousCar();

                selectionInputTimer = Time.time + 0.3f;
            } else if (horizontalAxis == 0)
                selectionInputTimer = 0;
        }
    }

    float selectionInputTimer;

    //selection menu events

    public void SelectNextCar() {
        selectedCarIndex++;
        if (selectedCarIndex == selectionMenuCars.Count)
            selectedCarIndex = 0;

        carModelsParent.transform.position = carModelsParentStartPosition - selectionCarPositionOffset * selectedCarIndex;
        UpdateMadeByText();
    }

    public void SelectPreviousCar() {
        selectedCarIndex--;
        if (selectedCarIndex < 0)
            selectedCarIndex = selectionMenuCars.Count - 1;

        carModelsParent.transform.position = carModelsParentStartPosition - selectionCarPositionOffset * selectedCarIndex;
        UpdateMadeByText();
    }

    private void UpdateMadeByText() {
        madeBy.text = "Made by " + selectionMenuCars[selectedCarIndex].CarDeveloperName;
    }
}
