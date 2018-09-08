using UnityEngine;
using System.Collections;

public class StartScreenScript : MonoBehaviour {

	static bool sawOnce = false;
    static bool start = false;

    public static void Init() {
        start = true;
    }

	// Use this for initialization
	void Start () {
		if(!sawOnce) {
			GetComponent<SpriteRenderer>().enabled = true;
			Time.timeScale = 0;
		}

		sawOnce = true;
	}
	
	// Update is called once per frame
	public void Update () {
        if(Time.timeScale==0 && start ) {
			Time.timeScale = 1;
			GetComponent<SpriteRenderer>().enabled = false;

		}
	}
}
