using UnityEngine;
using System.Collections;

public class SnakeBodyTrigger : MonoBehaviour {
	// OnTriggerEnter is our newest Unity event function.
	// Unity calls it if the script's GameObject has a trigger collider,
	// and some other object's collider enters the trigger collider volume.
	//
	// (Trigger collider means any collider component, such as
	// Sphere Collider, Box Collider etc. that has the Is Trigger
	// option enabled.)
	//
	// A moving trigger object should have a Rigidbody component as well,
	// otherwise the physics system does not deal well with it.
	// In this exercise, we use a Kinematic Rigidbody (Rigidbody with
	// the Is Kinematic option) because we don't want any actual physical
	// behavior like falling or bumping, just for the trigger to work.
	void OnTriggerEnter(Collider c) {
		GameObject go = GameObject.Find("GameManager");
		GameManager gm = go.GetComponent<GameManager>();
		gm.TriggerGameOver();
	}
}
