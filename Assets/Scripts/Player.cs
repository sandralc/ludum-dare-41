using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public float speed = 10f;
	public LayerMask blockingLayer;
	public float actionRange;

	private BoxCollider2D boxCollider;
	private Rigidbody2D rb2D;
	private Vector2 moveVelocity;

	private SpriteRenderer renderer;
	private Animator animator;

	[HideInInspector]public bool hidden = false;
	[HideInInspector]public bool cooking = false;

	public GameObject chocolateCake;
	public GameObject chocolateCookie;
	public GameObject chocolateShot;
	public GameObject crepes;
	public GameObject croissant;
	public GameObject flan;
	public GameObject strawberryTart;
	public GameObject sponge;
	public GameObject sugarCookies;
	public GameObject waffles;

	public AudioClip pickUpSound;
	public AudioClip pickUpSound2;
	public AudioClip hideSound;
	public AudioClip kitchenSound;
	public AudioClip placeObjectSound1;
	public AudioClip placeObjectSound2;

	// Use this for initialization
	void Start () {
		boxCollider = GetComponent<BoxCollider2D> ();
		rb2D = GetComponent<Rigidbody2D> ();
		rb2D.freezeRotation = true;
		renderer = GetComponent<SpriteRenderer> ();
		animator = GetComponent<Animator> ();
		Physics2D.queriesStartInColliders = false;
	}

	void Update() {
		if (!cooking) {
			WalkAnimation ();
			//Movement!
//		Vector2 move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
//		moveVelocity = move.normalized * speed;

//		float distance = move.magnitude;
//		Vector2 finalPosition = rb2D.position + moveVelocity * Time.fixedDeltaTime;
//		FaceTowardsMovingDirection (finalPosition);

			//Raycasting objects in vicinity!
			RaycastHit2D hitInfo = Physics2D.Raycast (transform.position, transform.up, actionRange);

			if (IsHideoutInFrontOfPlayer (hitInfo)) {
				if (Input.GetKeyUp (KeyCode.Return)) {
					if (!this.hidden)
						Hide ();
					else
						Unhide ();
				}
			}
			if (IsKitchenInFrontOfPlayer (hitInfo)) {
				if (Input.GetKeyUp (KeyCode.Return)) {
					HUDManager.instance.LaunchCooking ();
					SoundManager.instance.PlaySingle (kitchenSound);
					cooking = true;
				}
			}
			ThrowBait ();
		} else {
			if (Input.GetKeyDown (KeyCode.Escape)) {
				HUDManager.instance.CancelCooking ();
				cooking = false;
			}
		}
	}

	void WalkAnimation() {
		if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.D)) {
			animator.SetBool ("Walk", true);
		} else {
			animator.SetBool ("Walk", false);
		}
	}
	
	void FixedUpdate() {
		if (!hidden && !cooking) {
			transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * Time.deltaTime * speed, Space.World);
			transform.Translate(Vector3.up * Input.GetAxis("Vertical") * Time.deltaTime * speed, Space.World);
		}
//		if (!hidden)
//			rb2D.MovePosition (rb2D.position + moveVelocity * Time.fixedDeltaTime);
	}

	void FaceTowardsMovingDirection(Vector2 finalPosition) {
		var dir = finalPosition - new Vector2(transform.position.x, transform.position.y);
		var angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis (angle - 90, Vector3.forward);
	}

	bool IsHideoutInFrontOfPlayer(RaycastHit2D hit) {
		return hit.transform != null && hit.collider.tag == "Hideout";
	}

	bool IsKitchenInFrontOfPlayer(RaycastHit2D hit) {
		return hit.transform != null && hit.collider.tag == "Kitchen";
	}

	private void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "Ingredient") {
			other.gameObject.GetComponent<Ingredient> ().Collect ();
			SoundManager.instance.RandomizeSfx (pickUpSound, pickUpSound2);
			HUDManager.instance.UpdateIngredients ();
		} else if (other.name == "RoomTrigger") {
			RoomManager.instance.GoToRoom (int.Parse (other.tag.Split (" " [0]) [1]));
		}
	}

	void Hide() {
		renderer.enabled = false;
		this.hidden = true;
		SoundManager.instance.PlaySingle (hideSound);
	}

	void Unhide() {
		renderer.enabled = true;
		this.hidden = false;
		SoundManager.instance.PlaySingle (hideSound);
	}

	void ThrowBait() {
		if (Input.GetKeyDown (KeyCode.Alpha1) || Input.GetKeyDown (KeyCode.Keypad1)) {
			DropRecipe (0);
			SoundManager.instance.RandomizeSfx (placeObjectSound1, placeObjectSound2);
		} else if (Input.GetKeyDown (KeyCode.Alpha2) || Input.GetKeyDown (KeyCode.Keypad2)) {
			DropRecipe (1);
			SoundManager.instance.RandomizeSfx (placeObjectSound1, placeObjectSound2);
		} else if (Input.GetKeyDown (KeyCode.Alpha3) || Input.GetKeyDown (KeyCode.Keypad3)) {
			DropRecipe (2);
			SoundManager.instance.RandomizeSfx (placeObjectSound1, placeObjectSound2);
		} else if (Input.GetKeyDown (KeyCode.Alpha4) || Input.GetKeyDown (KeyCode.Keypad4)) {
			DropRecipe (3);
			SoundManager.instance.RandomizeSfx (placeObjectSound1, placeObjectSound2);
		}
	}

	void DropRecipe(int index) {
		Recipe.Type recipe = HUDManager.instance.GetCookedRecipeOnSlotIndex (index);
		if (recipe != null) {
			if (recipe.Equals (Recipe.Type.Sponge)) {
				Instantiate (sponge, transform.position, Quaternion.identity);
			} else if (recipe.Equals (Recipe.Type.ChocolateCake)) {
				Instantiate (chocolateCake, transform.position, Quaternion.identity);
			} else if (recipe.Equals (Recipe.Type.ChocolateCookies)) {
				Instantiate (chocolateCookie, transform.position, Quaternion.identity);
			} else if (recipe.Equals (Recipe.Type.Crepes)) {
				Instantiate (crepes, transform.position, Quaternion.identity);
			} else if (recipe.Equals (Recipe.Type.Croissant)) {
				Instantiate (croissant, transform.position, Quaternion.identity);
			} else if (recipe.Equals (Recipe.Type.Flan)) {
				Instantiate (flan, transform.position, Quaternion.identity);
			} else if (recipe.Equals (Recipe.Type.StrawberryTart)) {
				Instantiate (strawberryTart, transform.position, Quaternion.identity);
			} else if (recipe.Equals (Recipe.Type.SugarCookies)) {
				Instantiate (sugarCookies, transform.position, Quaternion.identity);
			} else if (recipe.Equals (Recipe.Type.Waffles)) {
				Instantiate (waffles, transform.position, Quaternion.identity);
			}
		}
	}

}
