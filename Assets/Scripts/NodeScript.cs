using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeScript : MonoBehaviour {

	public Transform secondNode;
	public Transform thirdNode;

	public float thisToSecondAngle;
	public float thisToThirdAngle;
	public float secondToThirdAngle;

	HUDScript hudS;
	PlayerController playerS;

	void Start () {
		CalculateAngles();
	}

	public void NodeStart () {
		hudS = GameObject.Find("HUD").GetComponent<HUDScript>();
		playerS = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
	}

	void OnTriggerEnter (Collider c) {
		if (c.tag == "Player") {
			if (playerS.GetNodeIndex() == int.Parse(name.Remove(0, 4))) {
				playerS.PassedNode(secondNode.position);
				if (name == "Node0") {
					hudS.LapUpdate();
				}
			}
		}

		if (c.tag == "NPC") {
			if (c.GetComponent<NPCController>() != null) {
				int index = c.GetComponent<NPCController>().nextNodeIndex;
				int currentNode = int.Parse(name.Remove(0, 4));

				if (index == currentNode) {
					c.GetComponent<NPCController>().SetNodePos(GetComponent<BoxCollider>().size.x);
				}
			} else if (c.GetComponent<NavMeshAgentController>() != null) {
				c.GetComponent<NavMeshAgentController>().SetNextPath();
				if (name == "Node0") {
					c.GetComponent<NavMeshAgentController>().UpdateLap();
				}
			}
		}
	}

	void CalculateAngles () {
		thisToSecondAngle = Vector3.Angle (transform.position, secondNode.position);
		thisToThirdAngle = Vector3.Angle(transform.position, thirdNode.position);
		secondToThirdAngle = Vector3.Angle(secondNode.position, thirdNode.position);
	}
}
