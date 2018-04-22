using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour {

	public GameObject gameManager;
	public GameObject roomManager;
	public GameObject soundManager;
	public HUDManager hudManager;
	public Player player;

	// Use this for initialization
	void Awake () {
		if (GameManager.instance == null)
			Instantiate (gameManager);
		GameManager.instance.player = player;

		GameManager.instance.hudManager = hudManager;

		if (RoomManager.instance == null)
			Instantiate (roomManager);

		if (SoundManager.instance == null)
			Instantiate (soundManager);
		
	}
}
