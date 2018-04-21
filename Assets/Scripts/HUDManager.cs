using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {

	public static HUDManager instance = null;

	private Text score;
	private Image[] recipeSlots;
	private Dictionary<Ingredient.Type, Text> ingredientSlots;

	// Use this for initialization
	void Awake () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
		DontDestroyOnLoad (gameObject);

		InitHUD ();
	}

	void InitHUD() {
		GameObject hud = GameObject.FindGameObjectWithTag ("HUD");
		score = hud.transform.Find ("Score").GetComponent<Text> ();

		recipeSlots = new Image[4];
		recipeSlots [0] = hud.transform.Find ("Cooked Recipes/Slot 1/Recipe").GetComponent<Image>();
		recipeSlots [1] = hud.transform.Find ("Cooked Recipes/Slot 2/Recipe").GetComponent<Image>();
		recipeSlots [2] = hud.transform.Find ("Cooked Recipes/Slot 3/Recipe").GetComponent<Image>();
		recipeSlots [3] = hud.transform.Find ("Cooked Recipes/Slot 4/Recipe").GetComponent<Image>();

		ingredientSlots = new Dictionary<Ingredient.Type, Text> ();

		ingredientSlots.Add(Ingredient.Type.Butter, hud.transform.Find ("Ingredients/Row/Butter").GetComponentInChildren<Text>());
		ingredientSlots.Add(Ingredient.Type.Sugar, hud.transform.Find ("Ingredients/Row/Sugar").GetComponentInChildren<Text>());
		ingredientSlots.Add(Ingredient.Type.Flour, hud.transform.Find ("Ingredients/Row/Flour").GetComponentInChildren<Text>());
		ingredientSlots.Add(Ingredient.Type.Egg, hud.transform.Find ("Ingredients/Row/Egg").GetComponentInChildren<Text>());
		ingredientSlots.Add(Ingredient.Type.Milk, hud.transform.Find ("Ingredients/Row2/Milk").GetComponentInChildren<Text>());
		ingredientSlots.Add(Ingredient.Type.Strawberry, hud.transform.Find ("Ingredients/Row2/Strawberry").GetComponentInChildren<Text>());
		ingredientSlots.Add(Ingredient.Type.Chocolate, hud.transform.Find ("Ingredients/Row2/Chocolate").GetComponentInChildren<Text>());
		ingredientSlots.Add(Ingredient.Type.Cream, hud.transform.Find ("Ingredients/Row2/Cream").GetComponentInChildren<Text>());
	}

	void Start() {
		UpdateScore ();
		UpdateIngredients ();
	}

	void UpdateScore() {
		score.text = "Score: " + GameManager.instance.score;
	}

	public void UpdateIngredients() {
		Dictionary<Ingredient.Type, int> collectedIngredients = GameManager.instance.collectedIngredients;
		foreach (KeyValuePair<Ingredient.Type, int> collectedIngredient in collectedIngredients) {
			ingredientSlots [collectedIngredient.Key].text = "x " + collectedIngredient.Value;
		}
	}

}
