using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Debug : MonoBehaviour {

	public Text debugText;
	Vector3 playerStartPos;
	GameObject player;
	Rigidbody rb;

	void Start () {
		player = GameObject.Find("Player");
		playerStartPos = player.transform.position;
		rb = player.GetComponent<Rigidbody>();
	}

	void Update () {
		
	}

	public void Reset () {
		player.transform.position = playerStartPos;
		player.transform.rotation = Quaternion.Euler(Vector3.zero);
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
	}

	void TextUpdate () {
		
	}
}
