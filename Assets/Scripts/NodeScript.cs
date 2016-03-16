using UnityEngine;
using System.Collections;

public class NodeScript : MonoBehaviour {

	void OnTriggerEnter (Collider c) {

		if (c.tag == "NPC") {
			int index = c.GetComponent<NPCController>().nextNodeIndex;
			int currentNode = int.Parse(name.Remove(0, 4));

			if (index == currentNode) {
				c.GetComponent<NPCController>().SetNodePos(GetComponent<BoxCollider>().size.x);
			}
		}
	}
}
