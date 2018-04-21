using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public float speed = 10f;
	public float rotationSpeed = 10f;
	public float detectionRange = 10;
	public float startWaitTime;

	public LineRenderer lineOfSight;
	public Gradient redColor;
	public Gradient greenColor;

	public Transform[] moveSpots;
	private int actualSpot = 0;
	private float waitTime;

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

		Physics2D.queriesStartInColliders = false;
	}
	
	// Update is called once per frame
	void Update () {
		
		RaycastHit2D hitInfo = Physics2D.Raycast (transform.position, transform.up, detectionRange);
		if (hitInfo.collider != null) {
			Debug.DrawLine (transform.position, hitInfo.point, Color.red);
			lineOfSight.SetPosition (1, hitInfo.point);
			lineOfSight.colorGradient = greenColor;
			if (hitInfo.collider.CompareTag ("Player") && !GameManager.instance.player.hidden) {
				Debug.Log ("Detected!");
				lineOfSight.colorGradient = redColor;
			}
		} else {
			Debug.DrawLine (transform.position, transform.position + transform.up * detectionRange);
			lineOfSight.SetPosition (1, transform.position + transform.up * detectionRange);
			lineOfSight.colorGradient = greenColor;
		}
			
		lineOfSight.SetPosition (0, transform.position);

		Patrol();
	}

	void Patrol() {
		transform.position = Vector2.MoveTowards (transform.position, moveSpots [actualSpot].position, speed * Time.deltaTime);

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
			var angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis (angle - 90, Vector3.forward), Time.deltaTime * rotationSpeed);
		}
	}
}
