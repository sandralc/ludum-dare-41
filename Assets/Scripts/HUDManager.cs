using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {

	public static HUDManager instance = null;
	public Sprite chocolateCake;
	public Sprite chocolateCookie;
	public Sprite chocolateShot;
	public Sprite crepes;
	public Sprite croissant;
	public Sprite flan;
	public Sprite strawberryTart;
	public Sprite sponge;
	public Sprite sugarCookies;
	public Sprite waffles;

	private Text score;
	private Image[] recipeSlots;
	private Dictionary<Ingredient.Type, Text> ingredientSlots;
	//Cooking panel specific
	private GameObject cookingPanel;
	private Dictionary<Ingredient.Type, Button> ingredientCookingButtons;
	private Dictionary<Ingredient.Type, Text> ingredientCookingButtonsText;
	private Text recipeText;
	private Button cookButton;

	private Dictionary<Ingredient.Type, int> recipe;
	private Dictionary<Recipe.Type, string> validRecipes;
	private List<Recipe.Type> cookedRecipes;

	// Use this for initialization
	void Awake () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
		DontDestroyOnLoad (gameObject);
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

		cookingPanel = hud.transform.Find ("Cooking Panel").gameObject;

		ingredientCookingButtons = new Dictionary<Ingredient.Type, Button> ();
		ingredientCookingButtonsText = new Dictionary<Ingredient.Type, Text> ();

		ingredientCookingButtons.Add(Ingredient.Type.Butter, cookingPanel.transform.Find ("Ingredients/Row/Butter").GetComponentInChildren<Button>());
		ingredientCookingButtonsText.Add(Ingredient.Type.Butter, cookingPanel.transform.Find ("Ingredients/Row/Butter").GetComponentInChildren<Text>());
		ingredientCookingButtons.Add(Ingredient.Type.Sugar, cookingPanel.transform.Find ("Ingredients/Row/Sugar").GetComponentInChildren<Button>());
		ingredientCookingButtonsText.Add(Ingredient.Type.Sugar, cookingPanel.transform.Find ("Ingredients/Row/Sugar").GetComponentInChildren<Text>());
		ingredientCookingButtons.Add(Ingredient.Type.Flour, cookingPanel.transform.Find ("Ingredients/Row/Flour").GetComponentInChildren<Button>());
		ingredientCookingButtonsText.Add(Ingredient.Type.Flour, cookingPanel.transform.Find ("Ingredients/Row/Flour").GetComponentInChildren<Text>());
		ingredientCookingButtons.Add(Ingredient.Type.Egg, cookingPanel.transform.Find ("Ingredients/Row/Egg").GetComponentInChildren<Button>());
		ingredientCookingButtonsText.Add(Ingredient.Type.Egg, cookingPanel.transform.Find ("Ingredients/Row/Egg").GetComponentInChildren<Text>());
		ingredientCookingButtons.Add(Ingredient.Type.Milk, cookingPanel.transform.Find ("Ingredients/Row2/Milk").GetComponentInChildren<Button>());
		ingredientCookingButtonsText.Add(Ingredient.Type.Milk, cookingPanel.transform.Find ("Ingredients/Row2/Milk").GetComponentInChildren<Text>());
		ingredientCookingButtons.Add(Ingredient.Type.Strawberry, cookingPanel.transform.Find ("Ingredients/Row2/Strawberry").GetComponentInChildren<Button>());
		ingredientCookingButtonsText.Add(Ingredient.Type.Strawberry, cookingPanel.transform.Find ("Ingredients/Row2/Strawberry").GetComponentInChildren<Text>());
		ingredientCookingButtons.Add(Ingredient.Type.Chocolate, cookingPanel.transform.Find ("Ingredients/Row2/Chocolate").GetComponentInChildren<Button>());
		ingredientCookingButtonsText.Add(Ingredient.Type.Chocolate, cookingPanel.transform.Find ("Ingredients/Row2/Chocolate").GetComponentInChildren<Text>());
		ingredientCookingButtons.Add(Ingredient.Type.Cream, cookingPanel.transform.Find ("Ingredients/Row2/Cream").GetComponentInChildren<Button>());
		ingredientCookingButtonsText.Add(Ingredient.Type.Cream, cookingPanel.transform.Find ("Ingredients/Row2/Cream").GetComponentInChildren<Text>());

		recipeText = cookingPanel.transform.Find ("Recipe/Recipe").GetComponent<Text> ();
		cookButton = cookingPanel.transform.Find ("Cook/Cook Button").GetComponent<Button> ();

		cookedRecipes = new List<Recipe.Type> ();
		recipe = new Dictionary<Ingredient.Type, int> ();

		cookButton.onClick.AddListener (() => {
			Recipe.Type validRecipe = EvaluateRecipe();
			if (validRecipe != Recipe.Type.None) {
				if (cookedRecipes.Count < 4) {
					cookedRecipes.Add(validRecipe);
					SubstractUsedIngredients();
					Debug.Log("Successfully cooked a new recipe! " + validRecipe.ToString());
					UpdateCookedRecipes();
					LaunchCooking();
				}
			}
		});

		InitIngredientCookingButtons ();
		UpdateIngredientCookingButtonsText ();
	}

	void Start() {
		InitHUD ();
		cookingPanel.SetActive (false);
		InitValidRecipes ();

		UpdateScore ();
		UpdateIngredients ();
		UpdateCookedRecipes ();
	}

	public void UpdateScore() {
		score.text = "Score: " + GameManager.instance.score;
	}

	public void UpdateIngredients() {
		Dictionary<Ingredient.Type, int> collectedIngredients = GameManager.instance.collectedIngredients;
		foreach (KeyValuePair<Ingredient.Type, int> collectedIngredient in collectedIngredients) {
			ingredientSlots [collectedIngredient.Key].text = "x " + collectedIngredient.Value;
		}
	}

	public void UpdateCookedRecipes() {
		int index = 0;
		foreach (Recipe.Type cookedRecipe in cookedRecipes) {
			Image recipePiec = recipeSlots [index].GetComponentInChildren<Image> ();
			recipePiec.gameObject.SetActive (true);
			recipePiec.sprite = GetSpriteForRecipeType(cookedRecipe);
			index++;
		}
		for (int i = index; i < recipeSlots.Length; i++) {
			recipeSlots [i].GetComponentInChildren<Image> ().gameObject.SetActive (false);
		}
	}

	public void SubstractUsedIngredients() {
		foreach (KeyValuePair<Ingredient.Type, int> ingredientSpent in recipe) {
			GameManager.instance.collectedIngredients [ingredientSpent.Key]--;
		}
	}

	public void LaunchCooking() {
		cookingPanel.SetActive (true);
		ClearRecipe ();
		UpdateIngredientCookingButtonsText ();
	}

	public void CancelCooking() {
		ClearRecipe ();
		cookingPanel.SetActive (false);
	}

	void InitIngredientCookingButtons() {
		foreach(KeyValuePair<Ingredient.Type, Button> button in ingredientCookingButtons) {
			button.Value.onClick.AddListener (() => {
				AddIngredientToRecipe(button.Key);
			});
		}
	}

	void UpdateIngredientCookingButtonsText() {
		Debug.Log ("Updating all the ingredieents");
		foreach (KeyValuePair<Ingredient.Type, Text> ingredientText in ingredientCookingButtonsText) {
			ingredientCookingButtonsText [ingredientText.Key].text = "x" + GameManager.instance.collectedIngredients [ingredientText.Key];
		}
	}

	void AddIngredientToRecipe(Ingredient.Type ingredient) {
		if (!recipe.ContainsKey (ingredient) && GameManager.instance.collectedIngredients[ingredient]>0) {
			recipe.Add (ingredient, 1);
			if (recipeText.text != "") {
				recipeText.text += ", ";
			}
			recipeText.text += ingredient.ToString().ToLower();
			ingredientCookingButtonsText [ingredient].text = "x" + (GameManager.instance.collectedIngredients [ingredient] - 1);
		}
	}

	void ClearRecipe () {
		if (recipe != null)
			recipe.Clear ();
		recipeText.text = "";
	}

	Recipe.Type EvaluateRecipe() {
		string recipeInTextFormat = RecipeToText ();
		foreach (KeyValuePair<Recipe.Type, string> validRecipe in validRecipes) {
			if (recipeInTextFormat.Equals (validRecipe.Value)) {
				return validRecipe.Key;
			}
		}
		return Recipe.Type.None;
	}

	string RecipeToText() {
		List<string> recipeInListFormat = new List<string> ();
		foreach (KeyValuePair<Ingredient.Type, int> recipeItem in recipe) {
			recipeInListFormat.Add (recipeItem.Key.ToString ());
		}
		recipeInListFormat.Sort ();

		string recipeInTextFormat = "";
		for (int i = 0; i < recipeInListFormat.Count; i++) {
			if (i != 0)
				recipeInTextFormat += ",";
			recipeInTextFormat += recipeInListFormat [i];
		}
		return recipeInTextFormat;
	}

	void InitValidRecipes() {
		validRecipes = new Dictionary<Recipe.Type, string> ();

		validRecipes.Add (Recipe.Type.ChocolateCake, "Chocolate,Egg,Flour,Sugar");
		validRecipes.Add (Recipe.Type.ChocolateCookies, "Butter,Chocolate,Flour,Sugar");
		validRecipes.Add (Recipe.Type.ChocolateShot, "Chocolate,Cream,Milk");
		validRecipes.Add (Recipe.Type.Crepes, "Butter,Cream,Egg,Flour,Sugar");
		validRecipes.Add (Recipe.Type.Croissant, "Butter,Egg,Flour,Sugar");
		validRecipes.Add (Recipe.Type.Flan, "Egg,Milk,Sugar");
		validRecipes.Add (Recipe.Type.StrawberryTart, "Butter,Flour,Milk,Strawberry,Sugar");
		validRecipes.Add (Recipe.Type.Sponge, "Egg,Flour,Sugar");
		validRecipes.Add (Recipe.Type.SugarCookies, "Butter,Egg,Flour,Milk,Sugar");
		validRecipes.Add (Recipe.Type.Waffles, "Butter,Cream,Egg,Flour,Strawberry,Sugar");
	}

	Sprite GetSpriteForRecipeType (Recipe.Type recipeType) {
		if (recipeType.Equals (Recipe.Type.ChocolateCake)) {
			return chocolateCake;
		} else if (recipeType.Equals (Recipe.Type.ChocolateCookies)) {
			return chocolateCookie;
		} else if (recipeType.Equals (Recipe.Type.ChocolateShot)) {
			return chocolateShot;
		} else if (recipeType.Equals (Recipe.Type.Crepes)) {
			return crepes;
		} else if (recipeType.Equals (Recipe.Type.Croissant)) {
			return croissant;
		} else if (recipeType.Equals (Recipe.Type.Flan)) {
			return flan;
		} else if (recipeType.Equals (Recipe.Type.Sponge)) {
			return sponge;
		} else if (recipeType.Equals (Recipe.Type.StrawberryTart)) {
			return strawberryTart;
		} else if (recipeType.Equals (Recipe.Type.SugarCookies)) {
			return sugarCookies;
		} else if (recipeType.Equals (Recipe.Type.Waffles)) {
			return waffles;
		}
		return chocolateCake;
	}

}
