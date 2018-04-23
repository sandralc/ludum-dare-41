using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;
	public static bool mainMenu = false;
	public HUDManager hudManager;

	public static Dictionary<Ingredient.Type, int> collectedIngredients;
	public static Dictionary<int, Sprite> collectedRecipePapers;

	[HideInInspector]public Player player;


	void Awake () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
		DontDestroyOnLoad (gameObject);
		InitGame ();
	}

	void InitGame() {

		if (collectedIngredients == null) {
			collectedIngredients = new Dictionary<Ingredient.Type, int> ();
			string[] ingredientTypeNames = System.Enum.GetNames (typeof(Ingredient.Type));
			for (int i = 0; i < ingredientTypeNames.Length; i++) {
				collectedIngredients.Add ((Ingredient.Type)System.Enum.Parse (typeof(Ingredient.Type), ingredientTypeNames [i]), 0);
			}
		}
		if (collectedRecipePapers == null) {
			collectedRecipePapers = new Dictionary<int, Sprite> ();
		}
	}

	public void GameOver() {
		int backwardsRoom = RoomManager.instance.GetCurrentRoom();
		if (backwardsRoom < 0)
			backwardsRoom = 0;
		RoomManager.room = backwardsRoom;
//		hudManager.GameOver ();
		SceneManager.LoadScene(0);
	}

	public void Win() {
		hudManager.Win ();
		RoomManager.room = 0;
		SceneManager.LoadScene(0);
	}

	void Respawn() {

		player.Spawn (player.respawnPoints [RoomManager.instance.GetCurrentRoom ()].transform.position);
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit ();
		}
	}
}
