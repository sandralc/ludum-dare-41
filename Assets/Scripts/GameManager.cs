using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;
	public HUDManager hudManager;

	public GameObject[] respawnPoints;

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
	
	// Update is called once per frame
	void Update () {
		
	}

	public void GameOver() {
		
		int backwardsRoom = RoomManager.instance.GetCurrentRoom() - 1;
		if (backwardsRoom < 0)
			backwardsRoom = 0;
		RoomManager.room = backwardsRoom;

		SceneManager.LoadScene(0);
	}

	void Respawn() {

		player.Spawn (respawnPoints [RoomManager.instance.GetCurrentRoom ()].transform.position);
	}
}
