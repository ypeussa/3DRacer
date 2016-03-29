using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public List<GameObject> cars;
	public GameObject carPicker;
	public Transform spawnPoint;
	public Transform cameraStart;

	public GameObject allModels;

	int carIndex;

	GameObject mainCam;
	public float gameStartCamSize;
	GameObject[] allNPCs;
	public float avoidanceDistance;
	public float timeScale;

	GameObject[] nodes;

	public bool spawnRandomNPC;
	public List<GameObject> npcPrefabs;
	GameObject[] spawns;

	[HideInInspector]
	public bool gameStart;

	void Start () {
		nodes = GameObject.FindGameObjectsWithTag("Node");
		spawns = GameObject.FindGameObjectsWithTag("NPCSpawn");
		mainCam = GameObject.FindGameObjectWithTag("MainCamera");
	}

	void SpawnNPC () {
		for (int i = 0; i < spawns.Length; i++) {
			GameObject npc = null;
			if (spawnRandomNPC) {
				int randomIndex = Random.Range(0, npcPrefabs.Count);
				while (cars[carIndex].GetComponent<IDScript>().id == npcPrefabs[randomIndex].GetComponent<IDScript>().id) {
					randomIndex = Random.Range(0, npcPrefabs.Count);
				}
				npc = (GameObject)Instantiate(npcPrefabs[randomIndex], spawns[i].transform.position, npcPrefabs[randomIndex].transform.rotation);
			} else {
				if (i > npcPrefabs.Count - 1) {
					if (cars[carIndex].GetComponent<IDScript>().id != npcPrefabs[i - npcPrefabs.Count].GetComponent<IDScript>().id) { // Ugly code
						npc = (GameObject)Instantiate(npcPrefabs[i - npcPrefabs.Count], spawns[i].transform.position, npcPrefabs[i - npcPrefabs.Count].transform.rotation);
					}
				} else {
					if (cars[carIndex].GetComponent<IDScript>().id != npcPrefabs[i].GetComponent<IDScript>().id) { // Ugly code
						npc = (GameObject)Instantiate(npcPrefabs[i], spawns[i].transform.position, npcPrefabs[i].transform.rotation);
					}
				}
			}
			if (npc != null) {
				npc.name = "NPC" + i;
				npc.transform.SetParent(GameObject.Find("NPCs").transform);
			}			
		}
	}

	void FixedUpdate () {
		/*
		if (gameStart) {
			//Time.timeScale = timeScale;
			for (int i = 0; i < allNPCs.Length; i++) {
				for (int j = 0; j < allNPCs.Length; j++) {
					if (allNPCs[i].gameObject != allNPCs[j].gameObject) {
						// If Wheel Collider NPCs
						if (allNPCs[i].GetComponent<NPCController>() != null && allNPCs[j].GetComponent<NPCController>() != null) {
							if (Vector3.Distance(allNPCs[i].transform.position, allNPCs[j].transform.position) < avoidanceDistance || allNPCs[i].GetComponent<NPCController>().deltaSpeed < 0.01f) {
								//allNPCs[i].GetComponent<NavMeshObstacle>().enabled = true;
								allNPCs[j].GetComponent<NavMeshObstacle>().enabled = true;

							} else {
								allNPCs[i].GetComponent<NavMeshObstacle>().enabled = false;
							}
						}
					}
				}
			}
		}
		*/
	}

	public void StartGame () {
		SpawnNPC();
		allNPCs = GameObject.FindGameObjectsWithTag("NPC");
		gameStart = true;
		Instantiate(cars[carIndex], spawnPoint.position, Quaternion.Euler(Vector3.zero));
		carPicker.SetActive(false);
		mainCam.transform.position = cameraStart.position;
		mainCam.transform.rotation = cameraStart.rotation;
		mainCam.GetComponent<Camera>().orthographicSize = gameStartCamSize;
		mainCam.GetComponent<CameraScript>().CameraFollow(true);
		for (int i = 0; i < allNPCs.Length; i++) {
			if (allNPCs[i].GetComponent<NPCController>() != null) {
				allNPCs[i].GetComponent<NPCController>().beginGame = true;
			} else if (allNPCs[i].GetComponent<NavMeshAgentController>() != null) {
				allNPCs[i].GetComponent<NavMeshAgentController>().SetNextPath();
			}
		}
		for (int i = 0; i < nodes.Length; i++) {
			nodes[i].GetComponent<NodeScript>().NodeStart();
		}
	}

	public void Next () {
		if (carIndex < cars.Count - 1) {
			allModels.transform.position += new Vector3(-15, 0, 0);
			carIndex++;
		}
	}

	public void Previous () {
		if (carIndex > 0) {
			carIndex--;
			allModels.transform.position += new Vector3(15, 0, 0);
		}
	}
}
