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
	[HideInInspector]public bool chased = false;

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
	public GameObject pannatriffle;

	public AudioClip pickUpSound;
	public AudioClip pickUpSound2;
	public AudioClip hideSound;
	public AudioClip kitchenSound;
	public AudioClip placeObjectSound1;
	public AudioClip placeObjectSound2;
	public AudioClip aahSound;

	[HideInInspector]public GameObject[] respawnPoints;

	// Use this for initialization
	void Start () {
		boxCollider = GetComponent<BoxCollider2D> ();
		rb2D = GetComponent<Rigidbody2D> ();
		rb2D.freezeRotation = true;
		renderer = GetComponent<SpriteRenderer> ();
		animator = GetComponent<Animator> ();
		Physics2D.queriesStartInColliders = false;

		GameObject[] allRespawnPoints = GameObject.FindGameObjectsWithTag ("Respawn Point");
		respawnPoints = new GameObject[allRespawnPoints.Length];
		for (int i = 0; i < allRespawnPoints.Length; i++) {
			var index = int.Parse(allRespawnPoints [i].name.Split (" "[0]) [1]);
			respawnPoints [index] = allRespawnPoints [i];
		}

		transform.position = respawnPoints [RoomManager.room].transform.position;
		RoomManager.instance.GoToRoom (RoomManager.room);
	}

	void Update() {
		if (!cooking && !chased) {
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
					GameManager.instance.hudManager.LaunchCooking ();
					SoundManager.instance.PlaySingle (kitchenSound);
					cooking = true;
				}
			}
		} else if (cooking){
			if (Input.GetKeyDown (KeyCode.Escape)) {
				GameManager.instance.hudManager.CancelCooking ();
				cooking = false;
			}
		}
		if (!cooking && !hidden) {
			ThrowBait ();
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
			GameManager.instance.hudManager.UpdateIngredients ();
		} else if (other.name == "RoomTrigger") {
			RoomManager.instance.GoToRoom (int.Parse (other.tag.Split (" " [0]) [1]));
		} else if (other.tag == "RecipePaper") {
			string recipePaperName = other.name;
			int recipePaperIndex = int.Parse(recipePaperName.Split (" "[0]) [1]);
			if (!GameManager.collectedRecipePapers.ContainsKey (recipePaperIndex)) {
				GameManager.collectedRecipePapers.Add (recipePaperIndex, other.gameObject.GetComponent<SpriteRenderer> ().sprite);
			}
			GameManager.instance.hudManager.UpdateRecipePapersScore ();
			SoundManager.instance.PlaySingle (aahSound);
			Destroy (other.gameObject);
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
		Recipe.Type recipe = GameManager.instance.hudManager.GetCookedRecipeOnSlotIndex (index);
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
			} else if (recipe.Equals (Recipe.Type.ChocolateShot)) {
				Instantiate (chocolateShot, transform.position, Quaternion.identity);
			} else if (recipe.Equals (Recipe.Type.Pannatriffle)) {
				Instantiate (pannatriffle, transform.position, Quaternion.identity);
			}
		}
	}

	public void Spawn(Vector3 spawnPosition) {
		transform.position = spawnPosition;
	}

}
