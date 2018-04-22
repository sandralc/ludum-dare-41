using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour {

	public GameObject gameManager;
	public GameObject hudManager;
	public GameObject roomManager;
	public GameObject soundManager;
	public Player player;

	// Use this for initialization
	void Awake () {
		if (GameManager.instance == null)
			Instantiate (gameManager);
		GameManager.instance.player = player;

		if (HUDManager.instance == null)
			Instantiate (hudManager);
		
	}
}
