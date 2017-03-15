using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CarLapSystemEvent : UnityEvent<CarLapSystem> { }

public class CarLapSystem : MonoBehaviour {
    public CarLapSystemEvent OnLapFinishedEvent;
    public int lap { get; private set; }

    public NodeScript[] nodes;
    Vector3 nextNodePos;

    public NodeScript currentNode { get { return nodes[nodeIndex]; } }
    public int nodeIndex { get; private set; }
    public float currentLapTime {get;private set;}
    public float bestLapTime { get; private set; }

    void Awake() {
        //find and sort nodes
        nodes = FindObjectsOfType<NodeScript>();
        System.Array.Sort(nodes, (x, y) => x.index - y.index);

        bestLapTime = float.MaxValue;
    }

    public Vector3 GetNextNodePos() {
        return nextNodePos;
    }

    private void Update() {
        currentLapTime += Time.deltaTime;
    }

    void OnTriggerEnter(Collider c) {
        if (c.tag == "Node") {
            var node = c.GetComponent<NodeScript>();
            if (node.index == nodeIndex) {
                //next node
                nodeIndex++;
                if (nodeIndex == nodes.Length) nodeIndex = 0;

                nextNodePos = node.secondNode.transform.position;

                if (node.index == 0) {
                    
                    //record lap time
                    if (lap > 0 && currentLapTime < bestLapTime)
                        bestLapTime = currentLapTime;

                    currentLapTime = 0;
                    lap++;

                    OnLapFinishedEvent.Invoke(this);
                }
            }
        }
    }
}