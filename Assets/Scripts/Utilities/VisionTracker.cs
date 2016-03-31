﻿using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

// VisionTracker will record how often a player is looking at this GameObject, and how much of a focus it has had recently.

// Attach VisionTracker to any GameObject if you ever want to get information about whether the player has been looking at it a lot.
// See the "vision test" scenes for a demo.

public class VisionTracker : MonoBehaviour
{
	//How often (in seconds) VisionTracker will check what is looking at what.
	public static float interval = 0.5f;

	//directLookAtScore will only increment when the lookAtScore is at least this much.
	public float lookAtThreshold=0.8f;

	//how many seconds of history VisionTracker will record.
	public float lookAtHistorySeconds=5;

	private float lookAtTimer = 0;
	private float directLookAtScore = 0;
	private GameObject player;
	private float[] lookAtHistory;
	private int historyIndex=0;
	private WaitForSeconds waitForSeconds;

	void Start ()
	{
		waitForSeconds = new WaitForSeconds (interval);
		player = Camera.main.gameObject;
		lookAtHistory= new float[Mathf.CeilToInt(lookAtHistorySeconds/interval)];
		StartCoroutine (Cycle());
	}

	public float GetHistoryScore(){
		//returns a number between [-1,1] where 1 means the player has been directly looking at this object for lookAtHistorySeconds seconds.
		// 0.6 means the player has been somewhat looking at it in these past lookAtHistorySeconds seconds.
		float total = 0;
		foreach (float i in lookAtHistory)
			total += i;
		return total/lookAtHistorySeconds;
	}

	public float GetLookAtScore(){
		//returns a score between [-1,1] where 1 means the player is looking directly at this object right now, -1 means directly looking away.
		Vector3 playerToObject = gameObject.transform.position - player.transform.position;
		playerToObject.Normalize ();
		Vector3 lookDirection = Camera.main.transform.forward;

		return Vector3.Dot (playerToObject, lookDirection);
	}

	public float GetDirectLookAtScore(){
		//The total number of seconds that the player has been directly looking at this object.
		//"Directly" means having a greater lookAtScore than the threshold value.
		return directLookAtScore;
	}

	private IEnumerator Cycle ()
	{
		yield return new WaitForSeconds (Random.Range (0, interval));
		while (true) {
			float score=GetLookAtScore();
			if (score>lookAtThreshold)
				directLookAtScore+=interval;

			lookAtHistory[historyIndex]=score;
			historyIndex++;
			historyIndex=historyIndex%lookAtHistory.Length;

			yield return waitForSeconds;

		}
	}
}
