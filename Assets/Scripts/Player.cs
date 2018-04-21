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

	public bool hidden = false;

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
	}

	void WalkAnimation() {
		if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.D)) {
			animator.SetBool ("Walk", true);
		} else {
			animator.SetBool ("Walk", false);
		}
	}
	
	void FixedUpdate() {
		if (!hidden) {
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

	private void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "Ingredient") {
			other.gameObject.GetComponent<Ingredient> ().Collect ();
			HUDManager.instance.UpdateIngredients ();
		} else if (other.name == "RoomTrigger") {
			RoomManager.instance.GoToRoom (int.Parse(other.tag.Split(" "[0])[1]));
		}
	}

	void Hide() {
		renderer.enabled = false;
		this.hidden = true;
	}

	void Unhide() {
		renderer.enabled = true;
		this.hidden = false;
	}

}
