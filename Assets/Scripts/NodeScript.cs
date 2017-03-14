﻿using UnityEngine;

public class NodeScript : MonoBehaviour {
    public NodeScript secondNode;
    public int nodeIndex = 0;

    HUDScript hudS;
    PlayerController player;

    public void Init(PlayerController player) {
        hudS = GameObject.Find("HUD").GetComponent<HUDScript>();
        this.player = player;
    }

    void OnTriggerEnter(Collider c) {
        if (c.tag == "Player") {
            if (player.GetNodeIndex() == nodeIndex) {
                player.PassNode(secondNode.transform.position);
                if (nodeIndex == 0) {
                    hudS.RecordLap();
                }
            }
        }
    }
}
