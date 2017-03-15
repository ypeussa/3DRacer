using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;

public class GameController : MonoBehaviour {
    public List<PlayerCar> playerCarPrefabs;
    public List<AICar> AICarPrefabs;
    public GameObject carSelectionMenu;
    public Transform spawnPoint;
    public Transform cameraStart;
    public Transform carModelsParent;
    public HUD HUD;
    public float gameStartCamSize;
    public float avoidanceDistance;
    public float timeScale;
    public Text madeBy;
    public int raceLapCount = 5;
    public UnityEvent OnRaceStartEvent;
    public CarLapSystemEvent OnCarFinishedRaceEvent;
    public int DEBUG_AmountOfAICars = 1000;

    float selectionInputTimer;
    Vector3 carModelsParentStartPosition;
    int selectedCarIndex;
    CameraSystem mainCam;
    Vector3 selectionCarPositionOffset = new Vector3(15, 0, 0);
    List<PlayerCar> selectionMenuCars;
    List<AICar> AICars;
    GameObject[] spawnPoints;
    PlayerCar player;

    void Start() {
        spawnPoints = GameObject.FindGameObjectsWithTag("NPCSpawn");
        mainCam = FindObjectOfType<CameraSystem>();

        carModelsParentStartPosition = carModelsParent.transform.position;
        CreateSelectionCarModels();

        UpdateMadeByText();
    }

    void CreateSelectionCarModels() {
        selectionMenuCars = new List<PlayerCar>();

        for (int i = 0; i < playerCarPrefabs.Count; i++) {
            selectionMenuCars.Add(Instantiate(playerCarPrefabs[i], carModelsParent));
            selectionMenuCars[i].SetAsSelectionMenuCar(selectionCarPositionOffset * i, Quaternion.LookRotation(new Vector3(-0.5f, -0.2f, -1)));
        }
    }

    void CreateAICars(int skipCarIndex) {
        var npcParent = GameObject.Find("NPCs").transform;
        AICars = new List<AICar>();

        for (int i = 0; i < spawnPoints.Length; i++) {
#if UNITY_EDITOR
            if (i == DEBUG_AmountOfAICars) break;
#endif
            List<float> accelerationMultipliers = new List<float>();

            for (int j = 0; j < AICarPrefabs.Count - 1; j++) {
                accelerationMultipliers.Add(0.7f + (1f - (j / (float)AICarPrefabs.Count)) * 0.3f);
            }

            if (i < AICarPrefabs.Count && i != skipCarIndex) {
                var aiCar = Instantiate(AICarPrefabs[i], spawnPoints[i].transform.position, AICarPrefabs[i].transform.rotation);
                aiCar.name = "NPC" + i;
                aiCar.transform.SetParent(npcParent);

                int randomIndex = Random.Range(0, accelerationMultipliers.Count);
                aiCar.MultiplyMaxAcceleration(accelerationMultipliers[randomIndex]);
                accelerationMultipliers.RemoveAt(randomIndex);

                AICars.Add(aiCar);
                aiCar.lapSystem.OnLapFinishedEvent.AddListener(OnCarLapCompleted);
            }
        }
    }

    public void StartRace() {
        //disable selection menu
        carSelectionMenu.SetActive(false);
        carModelsParent.gameObject.SetActive(false);

        //spawn AI cars and the player car
        CreateAICars(selectedCarIndex);
        player = Instantiate(playerCarPrefabs[selectedCarIndex], spawnPoint.position, Quaternion.identity);
        player.lapSystem.OnLapFinishedEvent.AddListener(OnCarLapCompleted);

        //camera setup
        mainCam.Init(cameraStart.position, cameraStart.rotation, gameStartCamSize, player);

        //start countdown
        player.enabled = false;
        HUD.Init(player, this);
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
        player.enabled = true;

        //start AI cars
        for (int i = 0; i < AICars.Count; i++) {
            AICars[i].StartMoving();
        }

        yield return countDownWait;
        HUD.SetCountdownText("");
    }

    //input 
    void Update() {

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
        //selection menu input
        if (carSelectionMenu.activeSelf) {
            if (Input.GetButtonDown("Start")) {
                StartRace();
            }

            //using axis for controller support
            float horizontalAxis = Input.GetAxisRaw("Horizontal");

            if (horizontalAxis != 0 && selectionInputTimer < Time.time) {
                if (horizontalAxis > 0) SelectNextCar();
                else SelectPreviousCar();

                selectionInputTimer = Time.time + 0.3f;
            } else if (horizontalAxis == 0)
                selectionInputTimer = 0;
        } else {
            if (Input.GetButtonDown("Start")) {
                SceneManager.LoadScene("Main");
            }
        }
    }

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

    void UpdateMadeByText() {
        madeBy.text = "Made by " + selectionMenuCars[selectedCarIndex].GetComponent<CarInfo>().CarDeveloperName;
    }

    void OnCarLapCompleted(CarLapSystem lapSystem) {
        if (lapSystem.lap == raceLapCount + 1) {
            OnCarFinishedRaceEvent.Invoke(lapSystem);

            var player = lapSystem.GetComponent<PlayerCar>();
            if (player) {
                //set player to AI mode (quick hack!)
                player.enabled = false;
                player.GetComponent<CarMovement>().maxTurnAngle = 45f;
                player.GetComponent<UnityEngine.AI.NavMeshObstacle>().enabled = false;
                var AI = player.gameObject.AddComponent<AICar>();
                AI.nodeIndex = 1;
                AI.StartMoving();
                AI.disableInputOnImpactThreshold = 1000000f;
            }
        }
    }
}
