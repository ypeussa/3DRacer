using UnityEngine;
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
	GameObject[] allNPCs;
	public float avoidanceDistance;
	public float timeScale;

	void Start () {
		mainCam = GameObject.FindGameObjectWithTag("MainCamera");
		//Time.timeScale = 0;
		allNPCs = GameObject.FindGameObjectsWithTag("NPC");
		print(allNPCs.Length + " NPCs");
	}

	void FixedUpdate () {

		//Time.timeScale = timeScale;

		for (int i = 0; i < allNPCs.Length; i++) {
			for (int j = 0; j < allNPCs.Length; j++) {
				if (allNPCs[i].gameObject != allNPCs[j].gameObject) {
					if (Vector3.Distance(allNPCs[i].transform.position, allNPCs[j].transform.position) < avoidanceDistance || allNPCs[i].GetComponent<NPCController>().deltaSpeed < 0.01f) {
						allNPCs[i].GetComponent<NavMeshObstacle>().enabled = true;
						allNPCs[j].GetComponent<NavMeshObstacle>().enabled = true;

					} else {
						allNPCs[i].GetComponent<NavMeshObstacle>().enabled = false;
					}
				}
			}			
		}
	}

	public void StartGame () {
		Time.timeScale = 1;
		Instantiate(cars[carIndex], spawnPoint.position, Quaternion.Euler(Vector3.zero));
		carPicker.SetActive(false);
		mainCam.transform.position = cameraStart.position;
		mainCam.transform.rotation = cameraStart.rotation;
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
