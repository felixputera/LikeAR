using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleLikeHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void onLike() {
		Debug.Log("unlike");
	}

	public void onUnlike() {
		Debug.Log("like");
	}
}
