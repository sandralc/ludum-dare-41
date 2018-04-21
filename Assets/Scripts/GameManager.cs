using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;

	public Dictionary<Ingredient.Type, int> collectedIngredients;

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
		collectedIngredients = new Dictionary<Ingredient.Type, int>();
		string[] ingredientTypeNames = System.Enum.GetNames (typeof(Ingredient.Type));
		for (int i = 0; i < ingredientTypeNames.Length; i++) {
			collectedIngredients.Add ((Ingredient.Type) System.Enum.Parse (typeof(Ingredient.Type), ingredientTypeNames[i]), 0);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void GameOver() {
		//TODO - Restart current room
	}
}
