using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public enum State {Patrol, Eat, Follow};
	public enum PatrolStyle {Coverage, LookAround, Vigilance};

	public float speed = 10f;
	public float rotationSpeed = 10f;
	public float detectionRange = 10;
	public float startWaitTime;

	public PatrolStyle patrolStyle;

	public LineRenderer lineOfSight;
	public Gradient redColor;
	public Gradient greenColor;
	public Gradient juicyColor;

	public Transform[] moveSpots;
	private int actualSpot = 0;
	private float waitTime;

	public Recipe.Type[] favouriteDessert;
	public float affectedSpeed;
	public float affectedEyesight;
	public float startEatTime;
	private float eatTime;

	private float overallSpeed;
	private float overallDetectionRange;

	private State state = Enemy.State.Patrol;
	private GameObject food;
	private Vector2 foodPosition;

	public GameObject foodExclamation;
	public GameObject playerExclamation;
	public GameObject forkAndSpoon;

	public AudioClip eatSound;
	public AudioClip detectedSound;

	public float startAngleOscillation;
	private float angleOscillation;
	private float initialOrientation;
	private Vector3 initialPosition;

	public LayerMask enemiesIgnore;

	private BoxCollider2D boxCollider;
	private Rigidbody2D rb2D;
	private Animator animator;

	private Transform target;

	// Use this for initialization
	void Start () {
		boxCollider = GetComponent<BoxCollider2D> ();
		rb2D = GetComponent<Rigidbody2D> ();
		rb2D.freezeRotation = true;
		animator = GetComponent<Animator> ();
		target = GameObject.FindGameObjectWithTag ("Player").transform;
		waitTime = startWaitTime;
		eatTime = startEatTime;

		overallSpeed = speed;
		overallDetectionRange = detectionRange;
		initialOrientation = transform.rotation.eulerAngles.z;
		initialPosition = transform.position;

		Physics2D.queriesStartInColliders = false;
	}
	
	// Update is called once per frame
	void Update () {

		if (state.Equals (State.Patrol)) {
			RaycastHit2D hitInfo = Physics2D.Raycast (transform.position, transform.up, overallDetectionRange);
			if (hitInfo.collider != null && !hitInfo.collider.CompareTag("Ingredient") && !hitInfo.collider.CompareTag("RecipePaper") && !hitInfo.collider.tag.Contains("Room")) {
				Debug.DrawLine (transform.position, hitInfo.point, Color.red);
				lineOfSight.SetPosition (1, hitInfo.point);
				lineOfSight.colorGradient = greenColor;
				if (hitInfo.collider.CompareTag ("Player") && !GameManager.instance.player.hidden) {
					SoundManager.instance.PlaySingle (detectedSound);
					lineOfSight.colorGradient = redColor;
					state = State.Follow;
					RoomManager.instance.CantLeaveRoom ();
					GameManager.instance.player.chased = true;
				}
				if (hitInfo.collider.CompareTag ("Recipe")) {
					if (favouriteDessert.Length == 0) {
						EnemyIsLookingAtRecipe (hitInfo);
					} else {
						bool favouriteDessert = IsFavouriteDessert (hitInfo.collider.name);
						if (favouriteDessert || (!favouriteDessert && Random.value < 0.2)) {
							EnemyIsLookingAtRecipe (hitInfo);
						}
					}
				}
			} else {
				Debug.DrawLine (transform.position, transform.position + transform.up * overallDetectionRange);
				lineOfSight.SetPosition (1, transform.position + transform.up * overallDetectionRange);
				lineOfSight.colorGradient = greenColor;
			}
			
			lineOfSight.SetPosition (0, transform.position);
			Patrol ();
		} else if (state.Equals (Enemy.State.Eat)) {
			Eat ();
		} else if (state.Equals (Enemy.State.Follow)) {
			Chase ();
		}
	}

	void EnemyIsLookingAtRecipe(RaycastHit2D hitInfo) {
		lineOfSight.colorGradient = juicyColor;
		state = State.Eat;
		SoundManager.instance.PlaySingle (eatSound);
		GameManager.instance.player.chased = false;
//		RoomManager.instance.CanLeaveRoom ();
		food = hitInfo.transform.gameObject;
		foodPosition = hitInfo.point;
	}

	void Chase() {
		RaycastHit2D hitInfo = Physics2D.Raycast (transform.position, target.position, overallDetectionRange);

		if (hitInfo.collider != null && hitInfo.collider.CompareTag ("Recipe")) {
			if (favouriteDessert.Length == 0) {
				EnemyIsLookingAtRecipe (hitInfo);
			} else {
				bool favouriteDessert = IsFavouriteDessert (hitInfo.collider.name);
				if (favouriteDessert || (!favouriteDessert && Random.value < 0.2)) {
					EnemyIsLookingAtRecipe (hitInfo);
				}
			}
			lineOfSight.SetPosition (1, hitInfo.point);
			lineOfSight.colorGradient = juicyColor;
		} else {
			if (Vector2.Distance (transform.position, target.position) > .6) {
				transform.position = Vector2.MoveTowards (transform.position, target.position, speed * Time.deltaTime);

				var dir = new Vector3 (foodPosition.x, foodPosition.y, 0.0f) - transform.position;
				var angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.AngleAxis (angle - 90, Vector3.forward), Time.deltaTime * rotationSpeed);
			} else {
				GameManager.instance.GameOver ();
			}
			lineOfSight.SetPosition (1, target.position);
			lineOfSight.colorGradient = redColor;
		}
		lineOfSight.SetPosition (0, transform.position);
	}

	void Patrol() {
		switch (patrolStyle) {
		case PatrolStyle.Coverage:
			PatrolCoverage ();
			break;
		case PatrolStyle.LookAround:
			PatrolLookingAround ();
			break;
		case PatrolStyle.Vigilance:
			PatrolVigilance ();
			break;
		}
	}

	void PatrolCoverage() {
		transform.position = Vector2.MoveTowards (transform.position, moveSpots [actualSpot].position, overallSpeed * Time.deltaTime);

		if (Vector2.Distance (transform.position, moveSpots [actualSpot].position) < 0.2f) {
			if (waitTime <= 0) {
				actualSpot++;
				if (actualSpot >= moveSpots.Length) {
					actualSpot = 0;
				}
				waitTime = startWaitTime;
			} else {
				waitTime -= Time.deltaTime;
			}
		} else {
			var dir = moveSpots [actualSpot].position - transform.position;
//			if (angleOscillation < -startAngleOscillation) {
//				angleOscillation += 100f * Time.deltaTime;
//				Debug.Log (angleOscillation);
//			} else {
//				angleOscillation -= 100f * Time.deltaTime;
//				Debug.Log (angleOscillation);
//			}
			var angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis (angle - 90, Vector3.forward), Time.deltaTime * rotationSpeed);
		}
	}

	void PatrolLookingAround() {
		float angle = Mathf.Sin(Time.time) * startAngleOscillation + initialOrientation;
		transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);

		if (Vector2.Distance (transform.position, initialPosition) > 0.1f) {
			transform.position = Vector2.MoveTowards (transform.position, initialPosition, overallSpeed * Time.deltaTime);
		}
	}

	void PatrolVigilance() {
		
	}

	void Eat() {
		transform.position = Vector2.MoveTowards (transform.position, foodPosition, overallSpeed * Time.deltaTime);

		if (Vector2.Distance(transform.position, foodPosition) < 0.2f) {
			if (eatTime <= 0) {
				eatTime = startEatTime;
				state = State.Patrol;
				if (affectedSpeed < 0) {
					overallSpeed = Mathf.Abs (Mathf.Max (speed + affectedSpeed, speed / 2));
				} else {
					overallSpeed = Mathf.Abs (Mathf.Min (speed + affectedSpeed, speed * 2));
				}
				if (affectedEyesight < 0) {
					overallDetectionRange = Mathf.Abs(Mathf.Max(detectionRange + affectedEyesight, detectionRange/2));
				} else {
					overallDetectionRange = Mathf.Abs(Mathf.Min(detectionRange + affectedEyesight, detectionRange * 2));
				}
				Destroy (food);
				lineOfSight.enabled = true;
			} else {
				eatTime -= Time.deltaTime;
				if (lineOfSight.enabled)
					lineOfSight.enabled = false;
			}
		} else {
			var dir = new Vector3(foodPosition.x, foodPosition.y, 0.0f) - transform.position;
			var angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis (angle - 90, Vector3.forward), Time.deltaTime * rotationSpeed);
		}
	}

	bool IsFavouriteDessert(string name) {
		for(int i = 0; i < favouriteDessert.Length;i++) {
			if (name.Contains (favouriteDessert [i].ToString())) {
				return true;
			}
		}
		return false;
	}
}
