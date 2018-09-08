using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlappyBirdScreen : MonoBehaviour {

    public Camera cam;
	// Use this for initialization
	void Start () {
        //cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        Debug.Log(cam.name);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hitInfo))
            {
                Debug.Log("Object Hit is " + hitInfo.collider.gameObject.name);

                //If you want it to only detect some certain game object it hits, you can do that here
                if (ReferenceEquals(hitInfo.collider.gameObject, gameObject))
                {
                    StartScreenScript.Init();
                }
            }

        }
    }
}
