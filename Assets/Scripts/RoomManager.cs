using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour {

	public static RoomManager instance = null;
	public GameObject[] rooms;

	private int room = 0;

	// Use this for initialization
	void Awake () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
		DontDestroyOnLoad (gameObject);

		InitRooms ();
	}

	void InitRooms() {
		int index = 0;
		foreach (GameObject room in rooms) {
			if (index > 0) {
				room.SetActive (false);
			} else {
				room.SetActive (true);
			}
			index++;
		}
	}

	public void GoToRoom(int roomIndex) {
		Debug.Log ("Going to room " + roomIndex);
		rooms [room].SetActive (false);
		rooms [roomIndex].SetActive (true);
		room = roomIndex;
	}

}
