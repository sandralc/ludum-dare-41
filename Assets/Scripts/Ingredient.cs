using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour {

	public enum Type {Sugar, Flour, Egg, Butter, Chocolate, Cream, Milk, Strawberry};

	private Ingredient.Type ingredientType;
	private BoxCollider2D boxCollider;

	// Use this for initialization
	void Start () {
		SetIngredientType ();
		boxCollider = GetComponent<BoxCollider2D> ();
	}

	void SetIngredientType() {
		if (transform.name.Contains ("Sugar")) {
			ingredientType = Type.Sugar;
		} else if (transform.name.Contains ("Flour")) {
			ingredientType = Type.Flour;
		} else if (transform.name.Contains ("Egg")) {
			ingredientType = Type.Egg;
		}  else if (transform.name.Contains ("Butter")) {
			ingredientType = Type.Butter;
		} else if (transform.name.Contains ("Chocolate")) {
			ingredientType = Type.Chocolate;
		} else if (transform.name.Contains ("Cream")) {
			ingredientType = Type.Cream;
		} else if (transform.name.Contains ("Milk")) {
			ingredientType = Type.Milk;
		} else if (transform.name.Contains ("Strawberry")) {
			ingredientType = Type.Strawberry;
		}
	}

	public void Collect() {
		GameManager.collectedIngredients [ingredientType]++;
		Destroy (gameObject);
	}
}
