using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeScript : MonoBehaviour {

	public Transform secondNode;
	public Transform thirdNode;

	public float thisToSecondAngle;
	public float thisToThirdAngle;
	public float secondToThirdAngle;

	void Start () {
		CalculateAngles();
	}

	void OnTriggerEnter (Collider c) {

		if (c.tag == "NPC") {
			int index = c.GetComponent<NPCController>().nextNodeIndex;
			int currentNode = int.Parse(name.Remove(0, 4));

			if (index == currentNode) {
				c.GetComponent<NPCController>().SetNodePos(GetComponent<BoxCollider>().size.x);
			}
		}
	}

	void CalculateAngles () {
		thisToSecondAngle = Vector3.Angle (transform.position, secondNode.position);
		thisToThirdAngle = Vector3.Angle(transform.position, thirdNode.position);
		secondToThirdAngle = Vector3.Angle(secondNode.position, thirdNode.position);
	}
}
