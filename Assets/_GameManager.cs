using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	// This list holds all the snake body parts. The order in the list matters:
	// the game logic considers the first item in the list to be the snake's
	// "head" and the last item in the list to be the "tail".
	public List<GameObject> snakeBodies;
	public GameObject food;

	public GameObject scoreTextObject;
	Text scoreText;
	public GameObject notificationTextObject;
	Text notificationText;
	
	public GameObject highScoreHolderPrefab;
	GameObject highScoreHolder;
	HighScore hs;

	public GameObject snakeBodyContainer;

	int score;

	// We use this reference to a prefab to Instantiate new snake parts when the
	// snake grows after eating food.
	public GameObject snakeBodyPrefab;
	
	public float tickTime = 0.5f; // seconds

	public int levelWidth = 25;
	public int levelHeight = 15;

	float timeSinceLastTick = 0.0f; // seconds
	bool growOnNextTick = false;

	bool isGameOver = false;

	public enum Direction { Left, Right, Up, Down };
	public Direction movementDirection = Direction.Right;

	public Direction lastMoveDirection = Direction.Right;

	public void TriggerGameOver() {
		isGameOver = true;
		// The special character \n in a string is an end of line.
		// This allows us to split our notification text to two lines
		// exactly where we want; if we didn't care where the split is,
		// we could just enable word wrap for the Text component in the editor.
		notificationText.text = "Game Over!\nPress space to reset.";
	}

	void RandomizeFoodLocation() {
		// Currently food could appear under the snake.
		// Preventing that is one of the suggested pieces of extra work.
		int x = Random.Range(-levelWidth / 2, levelWidth / 2);
		int z = Random.Range(-levelHeight / 2, levelHeight / 2);
		food.transform.position = new Vector3(x, 0, z);
	}

	void SetScoreText() {
		scoreText.text = "Score: " + score + " Hi: " + hs.currentHighScore;
	}

	void Start () {
		score = 0;
		// Our high score object doesn't get destroyed when reloading the scene,
		// so if we just placed it in the scene, an additional copy of the object
		// would be loaded every time.
		// Instead, we have GameManager instantiate the object only if
		// it's not already in the scene.
		highScoreHolder = GameObject.Find("HighScoreHolder(Clone)");
		if (highScoreHolder == null) {
			highScoreHolder = (GameObject)Instantiate(highScoreHolderPrefab);
		}
		hs = highScoreHolder.GetComponent<HighScore>();
		notificationText = notificationTextObject.GetComponent<Text>();
		scoreText = scoreTextObject.GetComponent<Text>();
		SetScoreText();
		RandomizeFoodLocation();
	}
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {
			// Reset game - just need to load this scene again.
			// You can see the index numbers of the scenes in
			// the project Build Settings where you decide which
			// scenes are included in the build.
			Application.LoadLevel(0);
		}

		// When game is over, we don't want to keep running the game logic.
		// We could wrap the rest of the Update() function in an
		// if statement, but it's neater to just return (exit function)
		// here instead, so we don't have to indent all the code.
		if (isGameOver)
			return;

		if (Input.GetKeyDown(KeyCode.RightArrow)) {
			if (lastMoveDirection != Direction.Left) {
				movementDirection = Direction.Right;
			}
		} else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			if (lastMoveDirection != Direction.Right) {
				movementDirection = Direction.Left;
			}
		} else if (Input.GetKeyDown(KeyCode.UpArrow)) {
			if (lastMoveDirection != Direction.Down) {
				movementDirection = Direction.Up;
			}
		} else if (Input.GetKeyDown(KeyCode.DownArrow)) {
			if (lastMoveDirection != Direction.Up) {
				movementDirection = Direction.Down;
			}
		}

		// We initialize the new position to the snake's head
		var newPosition = snakeBodies[0].transform.position;

		timeSinceLastTick += Time.deltaTime;
		if (timeSinceLastTick > tickTime) {

			timeSinceLastTick -= tickTime;

			// And we modify the new position according to where the
			// snake is moving.
			if (movementDirection == Direction.Right) {
				newPosition += new Vector3(1.0f, 0.0f, 0.0f);
			} else if (movementDirection == Direction.Down) {
				newPosition += new Vector3(0.0f, 0.0f, -1.0f);
			} else if (movementDirection == Direction.Up) {
				newPosition += new Vector3(0.0f, 0.0f, 1.0f);
			} else {
				newPosition += new Vector3(-1.0f, 0.0f, 0.0f);
			}
			lastMoveDirection = movementDirection;

			if (!growOnNextTick) {
				// Move the snake.
				// We could move all the snake body pieces individually,
				// but choose to use a trick instead: just give the last piece
				// the physical position where the snake is going next.
				var snakeTail = snakeBodies[snakeBodies.Count - 1];
				snakeTail.transform.position = newPosition;
				// We still need to move the piece to the front of the list to
				// make it the snake's head (as far as the game logic is concerned).
				// We add a copy of the GameObject reference to the beginning of the list,
				// and remove the old occurrence at the back of the list.
				// Together those are the same as moving.
				snakeBodies.Insert(0, snakeTail);
				snakeBodies.RemoveAt(snakeBodies.Count - 1);
			} else {
				// The snake ate food on last tick, and now it grows.
				// Instead of moving anything, we create a new body part,
				// and insert it at the front of the list as the snake's new head.
				var newBody = (GameObject)Instantiate(snakeBodyPrefab);
				// We put all snake bodies under the same GameObject to keep the project neat
				newBody.transform.parent = snakeBodyContainer.transform;
				newBody.transform.position = newPosition;
				snakeBodies.Insert(0, newBody);
				growOnNextTick = false;
			}

			// Now food may randomly appear under the snake, so we'll
			// check for food collisions against every snake body part.
			// Since we're processing every item in the List, this is a
			// good place to use the 'foreach' loop instead of 'for'.

			var foodPos = food.transform.position;
			foreach (GameObject g in snakeBodies) {
				var snakePartPos = g.transform.position;
				float distance = Vector3.Distance(snakePartPos, foodPos);

				if (distance < Mathf.Epsilon) {
					// Snake eats
					growOnNextTick = true;
					score += 1;
					if (score > hs.currentHighScore) {
						hs.currentHighScore = score;
					}
					SetScoreText();
					RandomizeFoodLocation();
				}
			}
		}

	}
}
