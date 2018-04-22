using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomManager : MonoBehaviour {

	public static RoomManager instance = null;
	public GameObject[] rooms;
	private TilemapCollider2D[] roomDoorsTilemap;

	private int room = 0;
	private bool cantLeaveRoom = false;

	// Use this for initialization
	void Awake () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
		DontDestroyOnLoad (gameObject);

		InitRooms ();
		InitRoomDoorsTilemaps ();
		CanLeaveRoom ();
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

	void InitRoomDoorsTilemaps() {
		roomDoorsTilemap = new TilemapCollider2D[rooms.Length];
		for(int i = 0; i < rooms.Length; i++) {
			roomDoorsTilemap [i] = rooms [i].transform.Find ("Grid/Doors").GetComponent<TilemapCollider2D> ();
		}
	}

	public void GoToRoom(int roomIndex) {
		if (roomIndex != room) {
			Debug.Log ("Going to room " + roomIndex);
			rooms [room].SetActive (false);
			rooms [roomIndex].SetActive (true);
			room = roomIndex;
			CanLeaveRoom ();
		}
	}

	public void CantLeaveRoom() {
		cantLeaveRoom = true;
		roomDoorsTilemap [room].enabled = true;
	}

	public void CanLeaveRoom() {
		cantLeaveRoom = false;
		if (roomDoorsTilemap[room] != null)
			roomDoorsTilemap [room].enabled = false;
	}

}
