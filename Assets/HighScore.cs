using UnityEngine;
using System.Collections;

// This script just stores the high score data for GameManager
// to use. It will not get destroyed when a new scene is loaded
// (or in our case, when the same scene is reloaded).
public class HighScore : MonoBehaviour {
	public int currentHighScore;

	void Start () {
		DontDestroyOnLoad(gameObject);
	}	
}
